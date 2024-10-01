using System.Text.Json;
using Adam.Core.Interfaces;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Adam.Core.Services;

public sealed class RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
    : IRedisCacheService
{
    public async Task<T?> GetCacheValueAsync<T>(string key)
    {
        logger.LogInformation($"Attempting to retrieve cache value for key: {key}");

        var db = redis.GetDatabase();
        var value = await db.StringGetAsync(key);
        if (value.IsNullOrEmpty)
        {
            logger.LogInformation($"No cache value found for key: {key}");
            return default;
        }

        logger.LogInformation($"Cache hit for key: {key}. Deserializing value.");
        return JsonSerializer.Deserialize<T>(value);
    }

    public async Task SetCacheValueAsync<T>(string key, T value, TimeSpan expirationTime)
    {
        logger.LogInformation(
            $"Setting cache value for key: {key} with expiration time: {expirationTime.TotalMinutes} minutes");

        var db = redis.GetDatabase();
        var serializedValue = JsonSerializer.Serialize(value);

        var success = await db.StringSetAsync(key, serializedValue, expirationTime);
        if (success)
        {
            logger.LogInformation($"Cache value successfully set for key: {key}");
        }
        else
        {
            logger.LogError($"Failed to set cache value for key: {key}");
        }
    }
}