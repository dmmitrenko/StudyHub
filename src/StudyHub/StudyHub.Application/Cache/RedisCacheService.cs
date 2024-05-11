using Microsoft.Extensions.Options;
using StackExchange.Redis;
using StudyHub.Infrastructure.Services;
using StudyHub.Infrastructure.Settings;

namespace StudyHub.Application.Cache;
public class RedisCacheService : ICacheService
{
    private readonly IDatabase _cache;
    private readonly CacheSettings _cacheSettings;

    public RedisCacheService(
        IDatabase cache,
        IOptions<CacheSettings> cacheSettings)
    {
        _cache = cache;
        _cacheSettings = cacheSettings.Value;
    }

    public async Task<string> GetCachedReminderTitle(string key)
    {
        var data = await _cache.StringGetAsync(key);
        if (data.IsNullOrEmpty)
            return string.Empty;

        return data.ToString();
    }

    public async Task SetCachedReminderTitle(string key, string reminderTitle)
    {
        await _cache.StringSetAsync(key, reminderTitle, _cacheSettings.Expiry);
    }
}
