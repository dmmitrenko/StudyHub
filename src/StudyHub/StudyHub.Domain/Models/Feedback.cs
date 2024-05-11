namespace StudyHub.Domain.Models;
public class Feedback
{
    public string TutorName { get; set; }
    public string TutorSurname { get; set; }
    public string TutorMiddleName { get; set; }
    public string Text { get; set; }
    public ushort Rate { get; set; }
}
