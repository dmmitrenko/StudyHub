using Microsoft.Extensions.Options;
using StudyHub.Domain;
using StudyHub.Domain.Enums;
using StudyHub.Domain.Models;
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

    public async Task<CommandResult> HandleCommand(Message message, string parameter, CancellationToken cancellationToken = default)
    {
        var validationResult = CommandValidator.ValidateReminderCommand(parameter, _reminderCommand);

        if (!validationResult.IsValid)
        {
            return new CommandResult(false, CommandKey, Errors: validationResult.Errors);
        }

        return new CommandResult(true, CommandKey, parameter);
    }

    public async Task<CommandResult> HandleCommand(CallbackQuery callbackQuery, CancellationToken cancellationToken = default)
    {
        var parts = callbackQuery.Data.Split('_');
        var year = int.Parse(parts[1]);
        var month = int.Parse(parts[2]);
        var day = int.Parse(parts[3]);
        var hour = int.Parse(parts[4]);
        var minute = int.Parse(parts[5]);
        var seconds = int.Parse(parts[6]);
        var title = parts[7];
        var selectedTime = new DateTime(year, month, day, hour, minute, seconds);

        var reminder = new Reminder
        {
            ChatId = callbackQuery.Message.Chat.Id,
            SendTime = selectedTime,
            Text = title,
        };

        await _reminderRepository.AddReminder(reminder);

        return new CommandResult(true, CommandKey);
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
