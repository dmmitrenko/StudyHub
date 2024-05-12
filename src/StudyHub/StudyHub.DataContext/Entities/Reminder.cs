using Azure;
using Azure.Data.Tables;

namespace StudyHub.DataContext.Entities;
public class Reminder : ITableEntity
{
    public long ChatId { get; set; }
    public DateTime SendTime { get; set; }
    public string Text { get; set; }
    public string? Link { get; set; }
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
