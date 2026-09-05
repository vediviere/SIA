namespace SIA.AdminBff.Infrastructure.Http;

public interface ICorrelationIdAccessor
{
  Guid CorrelationId { get; }
}
