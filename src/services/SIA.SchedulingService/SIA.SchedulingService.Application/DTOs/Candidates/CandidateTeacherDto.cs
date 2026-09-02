namespace SIA.SchedulingService.Application.Interfaces.ExternalServices;

public sealed record CandidateTeacherDto
{
    public required Guid TeacherId { get; init; }
    public required string ProfessionalProfile { get; init; }
    public Guid? ProgramId { get; init; }
    public required int ContractHours { get; init; }
    public required bool Status { get; init; }
}