using StudyHub.Domain.Models;

namespace StudyHub.Infrastructure.Repositories;
public interface IReminderRepository
{
    Task AddReminder(Reminder reminder);
    Task DeleteReminder(Reminder reminder);
    Task<Reminder> GetReminders(long chatId, DateTime sendTime);
}
