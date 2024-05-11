using Microsoft.Extensions.Options;
using StudyHub.Domain;
using StudyHub.Domain.Enums;
using StudyHub.Infrastructure;
using StudyHub.Infrastructure.Models;
using StudyHub.Infrastructure.Repositories;
using StudyHub.Infrastructure.Settings;
using System.Globalization;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace StudyHub.Application.Handlers;
public class AddReminderCommandHandler : ICommandHandler
{
    private readonly ReminderCommandSettings _reminderCommand;
    private readonly IReminderRepository _reminderRepository;

    public AddReminderCommandHandler(
        IOptions<ReminderCommandSettings> reminderSettings,
        IReminderRepository reminderRepository)
    {
        _reminderCommand = reminderSettings.Value;
        _reminderRepository = reminderRepository;
    }

    public Commands CommandKey => Commands.Remind;

    public async Task<CommandResult> HandleCommand(Message message, Dictionary<string, string> parameters, CancellationToken cancellationToken = default)
    {
        var inlineKeyboard = GenerateCalendar();

        return new CommandResult(true, CommandKey, inlineKeyboard);
    }

    public InlineKeyboardMarkup GenerateCalendar()
    {
        var currentDate = DateTime.UtcNow;
        int year = currentDate.Year;
        int month = currentDate.Month;

        var daysInMonth = DateTime.DaysInMonth(year, month);
        var firstDayOfMonth = new DateTime(year, month, 1);

        // Визначаємо кількість рядків клавіатури
        int rows = (int)Math.Ceiling((7 + daysInMonth) / 7.0);
        InlineKeyboardButton[][] buttons = new InlineKeyboardButton[rows][];

        for (int i = 0, day = 1; i < rows; i++)
        {
            buttons[i] = new InlineKeyboardButton[7];  // 7 днів у тижні
            for (int j = 0; j < 7; j++)
            {
                int dayOfWeekIndex = (int)firstDayOfMonth.DayOfWeek;
                if (i == 0 && j < dayOfWeekIndex || day > daysInMonth)
                {
                    // Заповнюємо порожні клітинки невидимими кнопками
                    buttons[i][j] = InlineKeyboardButton.WithCallbackData(" ", "ignore");
                }
                else
                {
                    buttons[i][j] = InlineKeyboardButton.WithCallbackData(day.ToString(), $"choose_{year}_{month}_{day}");
                    day++;
                }
            }
        }

        return new InlineKeyboardMarkup(buttons);
    }
}
