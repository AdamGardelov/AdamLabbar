using Adam.Core.Interfaces;
using Adam.Core.MediatR.Requests.Queries;
using Adam.Core.Models;
using MediatR;

namespace Adam.Core.MediatR.Handlers;

public class GetProductHandler(IProductRepository productRepository) : IRequestHandler<GetProductQuery, Product?>
{
    public async Task<Product?> Handle(GetProductQuery query, CancellationToken cancellationToken)
    {
        return await productRepository.GetProductAsync(query.Id);
    }
}