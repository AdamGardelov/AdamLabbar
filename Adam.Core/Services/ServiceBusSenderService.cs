using System.Text.Json;
using Adam.Core.Interfaces;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;

namespace Adam.Core.Services;

public sealed class ServiceBusSenderService(ServiceBusClient serviceBusClient, ILogger<ServiceBusSenderService> logger)
    : IServiceBusSenderService
{
    public async Task SendMessageAsync<T>(T message, string queueName)
    {
        try
        {
            logger.LogInformation($"Attempting to send message to queue: {queueName}");

            var sender = serviceBusClient.CreateSender(queueName);
            var jsonMessage = JsonSerializer.Serialize(message);

            logger.LogInformation($"Serialized message: {jsonMessage}");

            var serviceBusMessage = new ServiceBusMessage(jsonMessage);
            await sender.SendMessageAsync(serviceBusMessage);

            logger.LogInformation($"Message successfully sent to queue: {queueName}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Failed to send message to queue: {queueName}");
            throw;
        }
    }
}