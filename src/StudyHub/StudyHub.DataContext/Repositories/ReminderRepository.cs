using AutoMapper;
using Azure;
using Azure.Data.Tables;
using StudyHub.Domain.Models;
using StudyHub.Infrastructure.Repositories;

namespace StudyHub.DataContext.Repositories;
public class ReminderRepository : IReminderRepository
{
    private const string TableName = nameof(Reminder);
    private readonly IMapper _mapper;

    public ReminderRepository(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task AddReminder(Reminder reminder)
    {
        var entity = _mapper.Map<DataContext.Entities.Reminder>(reminder);
        var tableClient = await GetTableClient();
        await tableClient.AddEntityAsync(entity);
    }

    public async Task DeleteReminder(Reminder reminder)
    {
        var entity = _mapper.Map<DataContext.Entities.Reminder>(reminder);
        var tableClient = await GetTableClient();
        await tableClient.DeleteEntityAsync(entity.PartitionKey, entity.RowKey, entity.ETag);
    }

    public async Task<Reminder> GetReminders(long chatId, DateTime sendTime)
    {
        var tableClient = await GetTableClient();
        string partitionKey = chatId.ToString();
        string rowKey = sendTime.ToString("o");

        try
        {
            var response = await tableClient.GetEntityAsync<Entities.Reminder>(partitionKey, rowKey);
            return _mapper.Map<Reminder>(response.Value);
        }
        catch (RequestFailedException e) when (e.Status == 404)
        {
            return null;
        }
    }

    private async Task<TableClient> GetTableClient(CancellationToken cancellationToken = default)
    {
        var connectionString = Environment.GetEnvironmentVariable("TableStorageConnectionString");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Table storage connection string is not configured.");
        }

        var serviceClient = new TableServiceClient(connectionString);

        var tableClient = serviceClient.GetTableClient(TableName);
        await tableClient.CreateIfNotExistsAsync(cancellationToken);
        return tableClient;
    }
}
