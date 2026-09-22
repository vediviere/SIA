namespace SIA.TeacherBff.Infrastructure.Http;

public interface ICorrelationAccessor
{
  Guid CorrelationId { get; }
}
