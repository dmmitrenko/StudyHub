using AutoMapper;
using Azure;
using Azure.Data.Tables;
using StudyHub.Domain.Models;
using StudyHub.Infrastructure.Repositories;
using Telegram.Bot.Types;

namespace StudyHub.DataContext.Repositories;
public class TutorFeedbackRepository : IFeedbackRepository
{
    private const string TableName = nameof(Feedback);
    private readonly IMapper _mapper;

    public TutorFeedbackRepository(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task AddTutorFeedback(Feedback feedback)
    {
        var entity = _mapper.Map<DataContext.Entities.Feedback>(feedback);
        var tableClient = await GetTableClient();
        await tableClient.AddEntityAsync(entity);
    }

    public async Task DeleteTutorFeedback(Feedback feedback)
    {
        var entity = _mapper.Map<DataContext.Entities.Feedback>(feedback);
        var tableClient = await GetTableClient();
        await tableClient.DeleteEntityAsync(entity.PartitionKey, entity.RowKey, entity.ETag);
    }

    public async Task<List<Feedback>> GetFeedbacksForTutor(string tutorFullName)
    {
        var tableClient = await GetTableClient();

        var feeedbacks = new List<Feedback>();

        try
        {
            await foreach (var entity in tableClient.QueryAsync<Entities.Feedback>(filter: $"PartitionKey eq '{tutorFullName}'"))
            {
                feeedbacks.Add(_mapper.Map<Feedback>(entity));
            }
        }
        catch (RequestFailedException e)
        {
            Console.WriteLine($"An error occurred: {e.Message}");
            return null;
        }

        return feeedbacks;
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
