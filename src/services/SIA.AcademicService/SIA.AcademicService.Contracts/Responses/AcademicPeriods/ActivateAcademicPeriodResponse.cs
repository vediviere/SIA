
namespace SIA.AcademicService.Contracts.Responses.AcademicPeriods;

public sealed record ActivateAcademicPeriodResponse
{
    public required Guid Id { get; init; }

    public required bool Status { get; init; }

    public required DateTime? UpdatedAtUtc { get; init; }

    public required Guid CorrelationId { get; init; }
}