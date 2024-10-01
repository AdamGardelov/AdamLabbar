using Adam.Core.Constants;
using Adam.Core.Interfaces;
using Adam.Core.MediatR.Requests.Commands;
using Adam.Core.MediatR.Requests.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Adam.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController(IMediator mediator, IServiceBusSenderService serviceBusSender)
    : ControllerBase
{
    [HttpGet("GetProducts")]
    public async Task<IActionResult> GetProducts(CancellationToken cancellationToken)
    {
        var products = await mediator.Send(new GetProductsQuery(), cancellationToken);
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProduct(int id, CancellationToken cancellationToken)
    {
        var product = await mediator.Send(new GetProductQuery
        {
            Id = id
        }, cancellationToken);

        return product == null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var product = await mediator.Send(model, cancellationToken).ConfigureAwait(false);

        //Send created product to service bus
        await serviceBusSender.SendMessageAsync(product, ProductConstants.QueueName);

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductCommand model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedProduct = await mediator.Send(model, cancellationToken);
        return updatedProduct == null ? NotFound() : Ok(updatedProduct);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id, CancellationToken cancellationToken)
    {
        var deleted = await mediator.Send(new DeleteProductCommand
        {
            Id = id
        }, cancellationToken);

        return deleted ? Ok() : NotFound();
    }
}