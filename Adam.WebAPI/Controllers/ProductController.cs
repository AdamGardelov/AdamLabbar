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
    public async Task<IActionResult> GetProducts()
    {
        var products = await mediator.Send(new GetProductsQuery());
        return Ok(products);
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await mediator.Send(new GetProductQuery
        {
            Id = id
        });
        
        return product == null ? NotFound() : Ok(product);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var product = await mediator.Send(model).ConfigureAwait(false);
        
        //Send created product to service bus
        await serviceBusSender.SendMessageAsync(product, ProductConstants.QueueName);
        
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductCommand model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var updatedProduct = await mediator.Send(model);
        return updatedProduct == null ? NotFound() : Ok(updatedProduct);
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var deleted = await mediator.Send(new DeleteProductCommand
        {
            Id = id
        });
        
        return deleted ? Ok() : NotFound();
    }
}
