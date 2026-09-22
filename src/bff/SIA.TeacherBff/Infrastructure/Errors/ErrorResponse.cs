namespace SIA.TeacherBff.Infrastructure.Errors;

public sealed record ErrorResponse
{
  public required string Code { get; init; }
  public required string Message { get; init; }
  public required Guid CorrelationId { get; init; }
}
