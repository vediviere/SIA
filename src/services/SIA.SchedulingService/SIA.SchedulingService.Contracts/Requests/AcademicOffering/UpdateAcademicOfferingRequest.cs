namespace SIA.SchedulingService.Contracts.Requests;

public sealed record UpdateAcademicOfferingRequest
{
    public required string OfferingStatus { get; init; }
}