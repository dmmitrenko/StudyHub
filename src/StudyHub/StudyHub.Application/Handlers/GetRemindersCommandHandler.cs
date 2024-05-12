using StudyHub.Domain.Enums;
using StudyHub.Infrastructure;
using StudyHub.Infrastructure.Models;
using StudyHub.Infrastructure.Repositories;
using Telegram.Bot.Types;

namespace StudyHub.Application.Handlers;
public class GetRemindersCommandHandler : ICommandHandler
{
    private readonly IReminderRepository _reminderRepository;

    public GetRemindersCommandHandler(IReminderRepository reminderRepository)
    {
        _reminderRepository = reminderRepository;
    }

    public Commands CommandKey => Commands.GetReminders;

    public async Task<CommandResult> HandleCommand(Message message, string parameter, CancellationToken cancellationToken = default)
    {
        var reminders = await _reminderRepository.GetReminders(message.Chat.Id);
        return new CommandResult(true, Commands.GetReminders, reminders);

    }

    public Task<CommandResult> HandleCommand(CallbackQuery message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<CommandResult> HandleCommand(Message message, object parameter, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
