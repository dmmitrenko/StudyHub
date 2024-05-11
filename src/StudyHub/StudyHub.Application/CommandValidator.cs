using StudyHub.Infrastructure.Models;
using StudyHub.Infrastructure.Settings;

namespace StudyHub.Application;
public static class CommandValidator
{
    public static ValidationResult ValidateReminderCommand(string parameter, ReminderCommandSettings commandParameters)
    {
        return new ValidationResult
        {
            IsValid = true,
        };
    }
}
