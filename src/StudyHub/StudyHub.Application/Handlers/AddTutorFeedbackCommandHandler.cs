using StudyHub.DataContext.Entities;
using StudyHub.Domain.Enums;
using StudyHub.Domain.Models;
using StudyHub.Infrastructure;
using StudyHub.Infrastructure.Models;
using StudyHub.Infrastructure.Repositories;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace StudyHub.Application.Handlers;
public class AddTutorFeedbackCommandHandler : ICommandHandler
{
    private readonly IFeedbackRepository _feedbackRepository;

    public AddTutorFeedbackCommandHandler(IFeedbackRepository feedbackRepository)
    {
        _feedbackRepository = feedbackRepository;
    }

    public Commands CommandKey => Commands.AddFeedback;

    public async Task<CommandResult> HandleCommand(Message message, string parameter, CancellationToken cancellationToken = default)
    {
        var parts = parameter.Split(' ');
        var feedback = new Domain.Models.Feedback
        {
            TutorName = parts[0],
            TutorSurname = parts[1],
            TutorMiddleName = parts[2]
        };

        return new CommandResult(true, Commands.AddFeedback, feedback);
    }

    public Task<CommandResult> HandleCommand(CallbackQuery message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public static InlineKeyboardMarkup GetRateSelection()
    {
        var inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            InlineKeyboardButton.WithCallbackData("1", "rate_1"),
            InlineKeyboardButton.WithCallbackData("2", "rate_2"),
            InlineKeyboardButton.WithCallbackData("3", "rate_3"),
            InlineKeyboardButton.WithCallbackData("4", "rate_4"),
            InlineKeyboardButton.WithCallbackData("5", "rate_5")
        });

        return inlineKeyboard;
    }

    public async Task<CommandResult> HandleCommand(Message message, object parameter, CancellationToken cancellationToken = default)
    {
        await _feedbackRepository.AddTutorFeedback(parameter as Domain.Models.Feedback);

        return new CommandResult(true, Commands.AddFeedback);
    }
}
