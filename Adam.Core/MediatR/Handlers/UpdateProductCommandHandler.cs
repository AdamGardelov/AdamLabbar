using Adam.Core.Interfaces;
using Adam.Core.MediatR.Requests.Commands;
using Adam.Core.Models;
using MediatR;

namespace Adam.Core.MediatR.Handlers;

internal sealed class UpdateProductCommandHandler(IProductRepository productRepository)
    : IRequestHandler<UpdateProductCommand, Product?>
{
    public async Task<Product?> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        return await productRepository.UpdateProductAsync(command, cancellationToken).ConfigureAwait(false);
    }
}