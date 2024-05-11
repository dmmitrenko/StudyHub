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

    public static InlineKeyboardMarkup GetMonthSelection()
    {
        var months = new[]
        {
        "Січень", "Лютий", "Березень",
        "Квітень", "Травень", "Червень",
        "Липень", "Серпень", "Вересень",
        "Жовтень", "Листопад", "Грудень"
    };

        var keyboard = new List<List<InlineKeyboardButton>>();
        for (int i = 0; i < months.Length; i += 3)
        {
            var row = new List<InlineKeyboardButton>
        {
            InlineKeyboardButton.WithCallbackData(months[i], $"month_{i + 1}"),
            InlineKeyboardButton.WithCallbackData(months[i + 1], $"month_{i + 2}"),
            InlineKeyboardButton.WithCallbackData(months[i + 2], $"month_{i + 3}")
        };
            keyboard.Add(row);
        }
        return new InlineKeyboardMarkup(keyboard);
    }

    public static InlineKeyboardMarkup GetDaySelection(int year, int month)
    {
        var daysInMonth = DateTime.DaysInMonth(year, month);
        var keyboard = new List<List<InlineKeyboardButton>>();
        for (int day = 1; day <= daysInMonth; day += 7)
        {
            var row = new List<InlineKeyboardButton>();
            for (int d = 0; d < 7 && day + d <= daysInMonth; d++)
            {
                row.Add(InlineKeyboardButton.WithCallbackData((day + d).ToString(), $"date_{year}_{month}_{day + d}"));
            }
            keyboard.Add(row);
        }
        return new InlineKeyboardMarkup(keyboard);
    }

    public static InlineKeyboardMarkup GetTimeSelection(int year, int month, int day)
    {
        var keyboard = new List<List<InlineKeyboardButton>>();
        for (int hour = 0; hour < 24; hour += 6)
        {
            var row = new List<InlineKeyboardButton>();
            for (int h = hour; h < hour + 6 && h < 24; h++)
            {
                row.Add(InlineKeyboardButton.WithCallbackData($"{h}:00", $"time_{year}_{month}_{day}_{h}_0"));
                row.Add(InlineKeyboardButton.WithCallbackData($"{h}:30", $"time_{year}_{month}_{day}_{h}_30"));
            }
            keyboard.Add(row);
        }
        return new InlineKeyboardMarkup(keyboard);
    }
}
