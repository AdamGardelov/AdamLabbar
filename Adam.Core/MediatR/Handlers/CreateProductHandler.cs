using Adam.Core.Interfaces;
using Adam.Core.MediatR.Requests.Commands;
using Adam.Core.Models;
using MediatR;

namespace Adam.Core.MediatR.Handlers;

public class CreateProductHandler(IProductRepository productRepository) : IRequestHandler<CreateProductCommand, Product>
{
    public async Task<Product> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        return await productRepository.CreateProductAsync(command).ConfigureAwait(false);
    }
}
