namespace Adam.Core.Interfaces;

public interface IServiceBusSenderService
{
    Task SendMessageAsync<T>(T message, string queueName);
}