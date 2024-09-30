using MediatR;

namespace Adam.Core.MediatR.Requests.Commands;

public class DeleteProductCommand : IRequest<bool>
{
    public required int Id { get; set; }
}