using Adam.Core.Data;
using Adam.Core.Interfaces;
using Adam.Core.MediatR.Handlers;
using Adam.Core.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

var host = new HostBuilder()
    .ConfigureAppConfiguration((context, config) =>
    {
        var env = context.HostingEnvironment.EnvironmentName;
        config.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{env}.json", true, true)
            .AddEnvironmentVariables();
    })
    .ConfigureFunctionsWebApplication(worker => worker.UseNewtonsoftJson())
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddDbContext<ProductDbContext>(options =>
            options.UseInMemoryDatabase("ProductInMemoryDb"));

        // Redis
        var redisConnectionString = configuration["RedisConnection"];
        if (redisConnectionString != null)
        {
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
        }

        services.RegisterRequestHandlers();
    })
    .Build();

host.Run();