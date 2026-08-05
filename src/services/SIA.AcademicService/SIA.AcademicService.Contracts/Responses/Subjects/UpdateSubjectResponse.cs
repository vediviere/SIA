

namespace SIA.AcademicService.Contracts.Responses.Subjects;

public sealed class UpdateSubjectResponse
{
    public required Guid Id { get; init; }
    public required Guid TenantId { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required int Semester { get; init; }
    public required int TheoryHours { get; init; }
    public required int PracticeHours { get; init; }
    public required int Credits { get; init; }
    public required bool Status { get; init; }
    public required DateTime? UpdatedAtUtc { get; init; }
    public required Guid CorrelationId { get; init; }
}