namespace SIA.AdminBff.Infrastructure.Errors;

public sealed record BffErrorResponse
{
  public required string Code { get; init; }
  public required string Message { get; init; }
  public required Guid CorrelationId { get; init; }
}
