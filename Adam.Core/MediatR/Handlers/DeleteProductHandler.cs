using Adam.Core.Interfaces;
using Adam.Core.MediatR.Requests.Commands;
using MediatR;

namespace Adam.Core.MediatR.Handlers;

public class DeleteProductHandler(IProductRepository productRepository) : IRequestHandler<DeleteProductCommand, bool>
{
    public async Task<bool> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        return await productRepository.DeleteProductAsync(command.Id).ConfigureAwait(false);
    }
}
