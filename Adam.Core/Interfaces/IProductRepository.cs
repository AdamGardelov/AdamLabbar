using Adam.Core.MediatR.Requests.Commands;
using Adam.Core.Models;

namespace Adam.Core.Interfaces;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetProductsAsync(CancellationToken cancellationToken);
    Task<Product?> GetProductAsync(int id, CancellationToken cancellationToken);
    Task<Product> CreateProductAsync(CreateProductCommand productCommand, CancellationToken cancellationToken);
    Task<Product?> UpdateProductAsync(UpdateProductCommand productCommand, CancellationToken cancellationToken);
    Task<bool> DeleteProductAsync(int id, CancellationToken cancellationToken);
}