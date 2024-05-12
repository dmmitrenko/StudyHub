using AutoMapper;
using StudyHub.Domain.Enums;
using StudyHub.Infrastructure;
using StudyHub.Infrastructure.Models;
using StudyHub.Infrastructure.Repositories;
using Telegram.Bot.Types;

namespace StudyHub.Application.Handlers;
public class GetTutorFeedbackCommandHandler : ICommandHandler
{
    private readonly IFeedbackRepository _feedbackRepository;

    public GetTutorFeedbackCommandHandler(
        IMapper mapper,
        IFeedbackRepository feedbackRepository)
    {
        _feedbackRepository = feedbackRepository;
    }

    public Commands CommandKey => Commands.GetFeedback;

    public async Task<CommandResult> HandleCommand(Message message, string parameter, CancellationToken cancellationToken = default)
    {
        parameter = string.Join("_", parameter.Split(" "));
        var feedbacks = await _feedbackRepository.GetFeedbacksForTutor(parameter);
        return new CommandResult(true, Commands.GetFeedback, feedbacks);
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
