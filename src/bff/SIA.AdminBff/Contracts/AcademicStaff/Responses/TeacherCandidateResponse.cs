namespace SIA.AdminBff.Contracts.AcademicStaff.Responses;

public sealed record TeacherCandidateResponse
{
  public required Guid TeacherId { get; init; }
  public required string ProfessionalProfile { get; init; }
  public Guid? ProgramId { get; init; }
  public required int ContractHours { get; init; }
  public required bool IsActive { get; init; }
}
