# Adams Lab Project


## Technologies Used
- **Azure Functions** - For building functions as API.
- **Azure Cache for Redis** - For caching.
- **Azure Service Bus** - For messaging.
- **Azure API Management** - For API management.
- **.NET Aspire** - Testing .NET Aspire.
- **ASP.NET Core** - For building the REST API.
- **MediatR** - For handling CQRS requests.
- **Swagger** - For API documentation and testing.

## Endpoints for the product function

| Method | Endpoint                  | Description                     |
|--------|---------------------------|---------------------------------|
| GET    | `/api/products`            | Retrieve all products           |
| GET    | `/api/products/{id}`       | Retrieve a specific product     |
| POST   | `/api/products`            | Create a new product            |
| PUT    | `/api/products/{id}`       | Update an existing product      |
| DELETE | `/api/products/{id}`       | Delete a product                |

## Endpoints for WebAPI

| Method | Endpoint         | Description                     |
|--------|------------------|---------------------------------|
| GET    | `/products/GetProducts`     | Retrieve all products           |
| GET    | `/products/{id}` | Retrieve a specific product     |
| POST   | `/products`      | Create a new product            |
| PUT    | `/products/{id}` | Update an existing product      |
| DELETE | `/products/{id}` | Delete a product                |


## Request body

### Create a Product / Update a Product

```json
{
  "name": "Product Name",
  "description": "Product Description",
  "price": 100.00
}
```

## Info

Needs connection strings for redis and service bus to work.

local.settings.json:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "AzureServiceBusConnectionString": "service-bus-connection-string"
  }
}
```

Might need to install azurite to run the Azure functions locally. You can install it using the following command:

```bash
npm install -g azurite
