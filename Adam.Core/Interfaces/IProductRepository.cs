using Adam.Core.MediatR.Requests;
using Adam.Core.MediatR.Requests.Commands;
using Adam.Core.Models;

namespace Adam.Core.Interfaces;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetProductsAsync();
    Task<Product?> GetProductAsync(int id);
    Task<Product> CreateProductAsync(CreateProductCommand productCommand);
    Task<Product?> UpdateProductAsync(UpdateProductCommand productCommand);
    Task<bool> DeleteProductAsync(int id);
}