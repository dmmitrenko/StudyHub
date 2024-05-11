using StudyHub.Domain.Models;
using StudyHub.Infrastructure.Repositories;

namespace StudyHub.DataContext.Repositories;
public class TutorFeedbackRepository : IFeedbackRepository
{
    public Task AddTutorFeedback(Feedback reminder)
    {
        throw new NotImplementedException();
    }

    public Task DeleteTutorFeedback(Feedback reminder)
    {
        throw new NotImplementedException();
    }

    public Task<Reminder> GetFeedbacksForTutor(string tutorFullName)
    {
        throw new NotImplementedException();
    }
}
