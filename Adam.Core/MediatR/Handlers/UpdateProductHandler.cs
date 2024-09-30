using Adam.Core.Interfaces;
using Adam.Core.MediatR.Requests.Commands;
using Adam.Core.Models;
using MediatR;

namespace Adam.Core.MediatR.Handlers;

public class UpdateProductHandler(IProductRepository productRepository) : IRequestHandler<UpdateProductCommand, Product?>
{
    public async Task<Product?> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        return await productRepository.UpdateProductAsync(command).ConfigureAwait(false);
    }
}
