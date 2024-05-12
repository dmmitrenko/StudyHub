using Azure;
using Azure.Data.Tables;

namespace StudyHub.DataContext.Entities;
public class Feedback : ITableEntity
{
    public string TutorName { get; set; }
    public string TutorSurname { get; set; }
    public string TutorMiddleName { get; set; }
    public string Text { get; set; }
    public int Rate { get; set; }
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
