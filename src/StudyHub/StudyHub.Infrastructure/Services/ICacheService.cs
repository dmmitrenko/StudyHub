using StudyHub.Domain.Models;

namespace StudyHub.Infrastructure.Services;
public interface ICacheService
{
    Task<string> GetCachedValue(string key);
    Task SetCachedValue(string key, string reminderTitle);
    Task SetFeedback(string key, Feedback feedback);
    Task<Feedback> GetFeedback(string key);
    Task DeleteKey(string key);
}
