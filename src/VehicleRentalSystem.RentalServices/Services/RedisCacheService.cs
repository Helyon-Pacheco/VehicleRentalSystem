using Newtonsoft.Json;
using StackExchange.Redis;
using VehicleRentalSystem.Core.Interfaces.Services;

namespace VehicleRentalSystem.RentalServices.Services;

public class RedisCacheService : IRedisCacheService
{
    private readonly IDatabase _database;

    public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
    }

    public async Task<T?> GetCacheValueAsync<T>(string key)
    {
        var value = await _database.StringGetAsync(key);

        if (!value.HasValue)
            return default;

        var json = value.ToString();
        return JsonConvert.DeserializeObject<T>(json);
    }

    public async Task SetCacheValueAsync<T>(string key, T value)
    {
        await _database.StringSetAsync(key, JsonConvert.SerializeObject(value));
    }

    public async Task RemoveCacheValueAsync(string key)
    {
        await _database.KeyDeleteAsync(key);
    }
}
