namespace SIA.AcademicStaffService.Contracts.Requests.DivisionHeads;

public sealed record CreateDivisionHeadRequest
{
    public required Guid ProgramId { get; init; }
    public required Guid PersonId { get; init; }
}