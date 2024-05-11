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

        var parameters = ParseParameters(message.Text);

        return await handler.HandleCommand(message, parameters, cancellationToken);
    }

    private Dictionary<string, string> ParseParameters(string messageText)
    {
        var parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var parts = Regex.Matches(messageText, @"(-\w+)\s+(?:""(.+?)""|(\S+))")
            .Cast<Match>()
            .Select(m => new { Key = m.Groups[1].Value.TrimStart('-'), Value = m.Groups[2].Success ? m.Groups[2].Value : m.Groups[3].Value });

        foreach (var part in parts)
        {
            parameters[part.Key] = part.Value;
        }

        return parameters;
    }
}
