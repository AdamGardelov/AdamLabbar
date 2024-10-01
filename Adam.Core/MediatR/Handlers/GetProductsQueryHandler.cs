using Adam.Core.Interfaces;
using Adam.Core.MediatR.Requests.Queries;
using Adam.Core.Models;
using MediatR;

namespace Adam.Core.MediatR.Handlers;

internal sealed class GetProductsQueryHandler(IProductRepository productRepository)
    : IRequestHandler<GetProductsQuery, IEnumerable<Product>?>
{
    public async Task<IEnumerable<Product>?> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        return await productRepository.GetProductsAsync(cancellationToken);
    }
}