using StudyHub.Domain.Enums;
using StudyHub.Infrastructure.Models;
using Telegram.Bot.Types;

namespace StudyHub.Infrastructure;
public interface ICommandHandler
{
    Commands CommandKey { get; }
    Task<CommandResult> HandleCommand(Message message, Dictionary<string, string> parameters, CancellationToken cancellationToken = default);
}
