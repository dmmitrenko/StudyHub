using StudyHub.Domain.Enums;

namespace StudyHub.Infrastructure.Models;
public record CommandResult(bool Success, Commands CommandType, object? Response = null, IEnumerable<string>? Errors = default);
