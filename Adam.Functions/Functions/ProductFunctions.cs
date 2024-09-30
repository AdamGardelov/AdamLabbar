using System.Net;
using System.Text.Json;
using Adam.Core.Constants;
using Adam.Core.MediatR.Requests.Commands;
using Adam.Core.MediatR.Requests.Queries;
using Adam.Core.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Adam.Functions.Functions;

public class ProductFunctions(IMediator mediator, ILogger<ProductFunctions> logger)
{
    [Function(nameof(GetProducts))]
    [OpenApiOperation(operationId: "getProducts", tags: ["products"], Summary = "Get all products", Description = "Retrieves a list of products.", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<Product>), Summary = "List of products", Description = "Returns a list of products in JSON format.")]
    public async Task<IActionResult> GetProducts([HttpTrigger(AuthorizationLevel.Function, "GET",
        Route = "products")] HttpRequest req)
    {
        var products = await mediator.Send(new GetProductsQuery());
        return new OkObjectResult(products);
    }

    [Function(nameof(CreateProduct))]
    [OpenApiOperation(operationId: "createProduct", tags: ["products"], Summary = "Create a new product", Description = "Creates a new product with the provided data", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(CreateProductCommand), Required = true, Description = "Product creation data")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Product), Summary = "Created product", Description = "Returns the created product in JSON format")]
    public async Task<IActionResult> CreateProduct([HttpTrigger(AuthorizationLevel.Function, "POST",
        Route = "product")] HttpRequest req)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync().ConfigureAwait(false);
        var createModel = JsonSerializer.Deserialize<CreateProductCommand>(requestBody);
        if (createModel == null)
        {
            return new BadRequestResult();
        }
        
        var product = await mediator.Send(createModel);
        
        return new OkObjectResult(product);
    }

    [Function(nameof(GetProduct))]
    [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "Product ID")]
    [OpenApiOperation(operationId: "getProductById", tags: ["products"], Summary = "Get product by ID", Description = "Retrieves a single product by its ID", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Product), Summary = "Product details", Description = "Returns the product in JSON format")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Product not found")]
    public async Task<IActionResult> GetProduct([HttpTrigger(AuthorizationLevel.Function, "GET",
        Route = "product/{id:int}")] HttpRequest req, int id)
    {
        var product = await mediator.Send(new GetProductQuery
        {
            Id = id
        });
        
        return product == null ? new NotFoundResult() : new OkObjectResult(product);
    }

    [Function(nameof(UpdateProduct))]
    [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "Product ID")]
    [OpenApiOperation(operationId: "updateProduct", tags: ["products"], Summary = "Update a product", Description = "Updates an existing product by its ID", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(UpdateProductCommand), Required = true, Description = "Product update data")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Product), 
        Summary = "Updated product", 
        Description = "Returns the updated product in JSON format.")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Product not found")]
    public async Task<IActionResult> UpdateProduct([HttpTrigger(AuthorizationLevel.Function, "PUT",
        Route = "product")] HttpRequest req)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync().ConfigureAwait(false);
        var updateModel = JsonSerializer.Deserialize<UpdateProductCommand>(requestBody);

        if (updateModel == null)
        {
            return new BadRequestResult();
        }
        
        var updatedProduct = await mediator.Send(updateModel);
        return updatedProduct == null ? new NotFoundResult() : new OkObjectResult(updatedProduct);
    }

    [Function(nameof(DeleteProduct))]
    [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "Product ID")]
    [OpenApiOperation(operationId: "deleteProduct", tags: ["products"], Summary = "Delete a product", Description = "Deletes a product by its ID", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Description = "Product deleted")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Product not found")]
    public async Task<IActionResult> DeleteProduct([HttpTrigger(AuthorizationLevel.Function, "DELETE",
        Route = "product/{id:int}")] HttpRequest req, int id)
    {
        var deleted = await mediator.Send(new DeleteProductCommand
        {
            Id = id
        });
        
        return deleted ? new OkResult() : new NotFoundResult();
    }
    
    [Function(nameof(ProcessProductFromServiceBus))]
    public Task ProcessProductFromServiceBus(
        [ServiceBusTrigger(ProductConstants.QueueName, Connection = "AzureServiceBusConnectionString")] Product product)
    {
        try
        {
            logger.LogInformation($"Got product with name {product.Name} and ID: {product.Id} from service bus.");
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to process message from Service Bus: {ex.Message}");
            throw;
        }

        return Task.CompletedTask;
    }
}
