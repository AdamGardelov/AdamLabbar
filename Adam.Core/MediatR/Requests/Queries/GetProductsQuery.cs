using Adam.Core.Models;
using MediatR;

namespace Adam.Core.MediatR.Requests.Queries;

public class GetProductsQuery : IRequest<IEnumerable<Product>?>;