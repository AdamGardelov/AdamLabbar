using Adam.Core.Models;
using MediatR;

namespace Adam.Core.MediatR.Requests.Queries;

public class GetProductQuery : IRequest<Product?>
{
    public required int Id { get; set; }
}