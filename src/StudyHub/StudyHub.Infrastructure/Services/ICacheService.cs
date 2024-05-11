namespace StudyHub.Infrastructure.Services;
public interface ICacheService
{
    Task<string> GetCachedReminderTitle(string key);
    Task SetCachedReminderTitle(string key, string reminderTitle);
}
