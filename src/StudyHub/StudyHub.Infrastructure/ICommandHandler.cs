using StudyHub.Domain.Enums;
using StudyHub.Infrastructure.Models;
using Telegram.Bot.Types;

namespace StudyHub.Infrastructure;
public interface ICommandHandler
{
    Commands CommandKey { get; }
    Task<CommandResult> HandleCommand(Message message, string parameter, CancellationToken cancellationToken = default);
    Task<CommandResult> HandleCommand(Message message, object parameter, CancellationToken cancellationToken = default);
    Task<CommandResult> HandleCommand(CallbackQuery message, CancellationToken cancellationToken = default);
}
