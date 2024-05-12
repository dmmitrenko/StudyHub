using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using StudyHub.Domain.Models;
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

    public async Task DeleteKey(string key)
    {
        _cache.KeyDelete(key);
    }

    public async Task<string> GetCachedValue(string key)
    {
        var data = await _cache.StringGetAsync(key);
        if (data.IsNullOrEmpty)
            return string.Empty;

        return data.ToString();
    }

    public async Task<Feedback> GetFeedback(string key)
    {
        var data = await _cache.StringGetAsync(key);
        if (data.IsNullOrEmpty)
            return new Feedback();

        return JsonConvert.DeserializeObject<Feedback>(data!)!;
    }

    public async Task SetCachedValue(string key, string reminderTitle)
    {
        await _cache.StringSetAsync(key, reminderTitle, _cacheSettings.Expiry);
    }

    public async Task SetFeedback(string key, Feedback feedback)
    {
        var feedbackJson = JsonConvert.SerializeObject(feedback);
        await _cache.StringSetAsync(key, feedbackJson, _cacheSettings.Expiry);
    }
}
