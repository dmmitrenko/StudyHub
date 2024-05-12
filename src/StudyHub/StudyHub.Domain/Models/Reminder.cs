namespace StudyHub.Domain.Models;
public class Reminder
{
    public long ChatId { get; set; }
    public DateTime SendTime { get; set; }
    public string Text { get; set; }
    public string? Link { get; set; }
}
