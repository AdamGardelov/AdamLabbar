using Adam.Core.Interfaces;
using Adam.Core.MediatR.Handlers;
using Adam.Core.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Extensions.Configuration;
using Adam.Core.Data;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var host = new HostBuilder()
    .ConfigureAppConfiguration((context, config) =>
    {
        var env = context.HostingEnvironment.EnvironmentName;
        config.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
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