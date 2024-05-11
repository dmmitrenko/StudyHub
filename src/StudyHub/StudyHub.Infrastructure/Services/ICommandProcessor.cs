using StudyHub.Domain.Enums;
using StudyHub.Infrastructure.Models;
using Telegram.Bot.Types;

namespace StudyHub.Infrastructure.Services;
public interface ICommandProcessor
{
    Task<CommandResult> HandleCommand(Message message, Commands command, CancellationToken cancellationToken = default);
    Task<CommandResult> HandleCommand(CallbackQuery message, Commands command, CancellationToken cancellationToken = default);
}
