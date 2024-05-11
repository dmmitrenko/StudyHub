using StudyHub.Domain.Enums;
using StudyHub.Infrastructure.Models;
using StudyHub.Infrastructure.Services;
using StudyHub.Infrastructure;
using System.Text.RegularExpressions;
using Telegram.Bot.Types;

namespace StudyHub.Application.Services;
public class CommandProcessor : ICommandProcessor
{
    private Dictionary<Commands, ICommandHandler> _commandHandlers;

    public CommandProcessor(IEnumerable<ICommandHandler> handlers)
    {
        _commandHandlers = handlers.ToDictionary(handler => handler.CommandKey);
    }

    public async Task<CommandResult> HandleCommand(Message message, Commands command, CancellationToken cancellationToken = default)
    {
        if (!_commandHandlers.TryGetValue(command, out var handler))
        {
            return new CommandResult(false, Commands.Undefined);
        }

        var parameter = ParseParameter(message.Text);

        return await handler.HandleCommand(message, parameter, cancellationToken);
    }

    public async Task<CommandResult> HandleCommand(CallbackQuery message, Commands command, CancellationToken cancellationToken = default)
    {
        if (!_commandHandlers.TryGetValue(command, out var handler))
        {
            return new CommandResult(false, Commands.Undefined);
        }

        return await handler.HandleCommand(message, cancellationToken);
    }

    private string ParseParameter(string messageText)
    {
        var parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var parts = messageText.Split(" ", 2);

        return parts[1];
    }
}
