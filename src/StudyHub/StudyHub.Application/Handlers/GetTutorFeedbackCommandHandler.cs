using AutoMapper;
using StudyHub.Domain.Enums;
using StudyHub.Infrastructure;
using StudyHub.Infrastructure.Models;
using Telegram.Bot.Types;

namespace StudyHub.Application.Handlers;
public class GetTutorFeedbackCommandHandler : ICommandHandler
{
    public GetTutorFeedbackCommandHandler(IMapper mapper)
    {
    }

    public Commands CommandKey => Commands.GetFeedback;

    public Task<CommandResult> HandleCommand(Message message, string parameter, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<CommandResult> HandleCommand(CallbackQuery message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
