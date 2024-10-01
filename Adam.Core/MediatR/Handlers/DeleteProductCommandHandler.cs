using Adam.Core.Interfaces;
using Adam.Core.MediatR.Requests.Commands;
using MediatR;

namespace Adam.Core.MediatR.Handlers;

internal sealed class DeleteProductCommandHandler(IProductRepository productRepository)
    : IRequestHandler<DeleteProductCommand, bool>
{
    public async Task<bool> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        return await productRepository.DeleteProductAsync(command.Id, cancellationToken).ConfigureAwait(false);
    }
}