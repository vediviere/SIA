namespace SIA.AcademicStaffService.Contracts.Responses.DivisionManagers;

public sealed record UpdateDivisionHeadResponse
{
    public required Guid Id { get; init; }

    public required Guid TenantId { get; init; }

    public required Guid ProgramId { get; init; }

    public required Guid PersonId { get; init; }

    public required bool Status { get; init; }

    public required DateTime? UpdatedAtUtc { get; init; }

    public required Guid CorrelationId { get; init; }
}