using Adam.Core.Constants;
using Adam.Core.Data;
using Adam.Core.Interfaces;
using Adam.Core.MediatR.Requests.Commands;
using Adam.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Adam.Core.Repositories;

public sealed class ProductRepository(
    ILogger<ProductRepository> logger,
    IRedisCacheService cacheService,
    ProductDbContext dbContext)
    : IProductRepository
{
    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        var cachedProducts = (await cacheService.GetCacheValueAsync<IEnumerable<Product>>(ProductConstants.ProductsCacheKey))?.ToList();
        if (cachedProducts != null && cachedProducts.Count != 0)
        {
            logger.LogInformation("Retrieved product list from cache.");
            return cachedProducts;
        }
        
        var products = await dbContext.Products.ToListAsync();
        await cacheService.SetCacheValueAsync(ProductConstants.ProductsCacheKey, products, TimeSpan.FromMinutes(10));

        logger.LogInformation("Product list cached.");
        return products;
    }

    public async Task<Product?> GetProductAsync(int id)
    {
        var cacheKey = $"Product_{id}";
        var cachedProduct = await cacheService.GetCacheValueAsync<Product>(cacheKey);
        if (cachedProduct != null)
        {
            logger.LogInformation($"Retrieved product with id {id} from cache.");
            return cachedProduct;
        }
        
        var product = await dbContext.Products.FindAsync(id);
        if (product != null)
        {
            await cacheService.SetCacheValueAsync(cacheKey, product, TimeSpan.FromMinutes(10));
            logger.LogInformation($"Product with id {id} cached.");
            return product;
        }

        logger.LogWarning($"Product with id {id} not found.");
        throw new KeyNotFoundException($"Product with ID {id} was not found.");
    }

    public async Task<Product> CreateProductAsync(CreateProductCommand productCommand)
    {
        var product = new Product
        {
            Id = productCommand.Id,
            Name = productCommand.Name,
            Price = productCommand.Price
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();
        
        logger.LogInformation($"Product with id {productCommand.Id} created.");
        
        // Cache the product in redis
        var cacheKey = $"Product_{productCommand.Id}";
        await cacheService.SetCacheValueAsync(cacheKey, product, TimeSpan.FromMinutes(10));
        
        await InvalidateProductListCacheAsync();

        return product;
    }

    public async Task<Product?> UpdateProductAsync(UpdateProductCommand productCommand)
    {
        var existingProduct = await dbContext.Products.FindAsync(productCommand.Id);
        if (existingProduct == null)
        {
            logger.LogWarning($"Product with id {productCommand.Id} not found for update.");
            throw new KeyNotFoundException($"Product with ID {productCommand.Id} was not found.");
        }

        if (!string.IsNullOrWhiteSpace(productCommand.Name))
        {
            existingProduct.Name = productCommand.Name;
        }

        if (productCommand.Price.HasValue)
        {
            existingProduct.Price = productCommand.Price.Value;
        }

        dbContext.Products.Update(existingProduct);
        await dbContext.SaveChangesAsync();
        
        logger.LogInformation($"Product with id {productCommand.Id} updated.");
        
        // Update the product in redis cache
        var cacheKey = $"Product_{productCommand.Id}";
        await cacheService.SetCacheValueAsync(cacheKey, existingProduct, TimeSpan.FromMinutes(10));
        
        await InvalidateProductListCacheAsync();

        return existingProduct;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = dbContext.Products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            logger.LogWarning($"Product with id {id} not found for deletion.");
            return false;
        }

        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync();
        
        // Remove the product from redis cache
        var cacheKey = $"Product_{id}";
        await cacheService.SetCacheValueAsync<Product?>(cacheKey, null, TimeSpan.Zero);

        await InvalidateProductListCacheAsync();

        logger.LogInformation($"Product with id {id} deleted.");

        return true;
    }

    #region Private help methods

    private async Task InvalidateProductListCacheAsync()
    {
        // Get the current list of products from db
        var updatedProductList = await dbContext.Products.ToListAsync();

        // Update the redis cache with the updated product list
        await cacheService.SetCacheValueAsync(ProductConstants.ProductsCacheKey, updatedProductList, TimeSpan.FromMinutes(10));

        logger.LogInformation("Product list cache invalidated and updated.");
    }

    #endregion Private help methods 
}