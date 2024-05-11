using StudyHub.Domain.Models;

namespace StudyHub.Infrastructure.Repositories;
public interface IFeedbackRepository
{
    Task AddTutorFeedback(Feedback reminder);
    Task DeleteTutorFeedback(Feedback reminder);
    Task<Reminder> GetFeedbacksForTutor(string tutorFullName);
}
