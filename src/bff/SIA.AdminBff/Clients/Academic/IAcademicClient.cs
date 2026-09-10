namespace SIA.AdminBff.Clients.Academic;

public interface IAcademicClient
{
  Task<AcademicContextDto> GetAcademicContextAsync(Guid tenantId, Guid educationalProgramId, CancellationToken cancellationToken);
}
