namespace StudyHub.DataContext.Entities;
public class TimeTable
{
    public TimeSpan Time { get; set; }
    public DayOfWeek Day { get; set; }
    public string Subject { get; set; }
}
