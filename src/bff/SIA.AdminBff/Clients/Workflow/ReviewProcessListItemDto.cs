namespace SIA.AdminBff.Clients.Workflow;

public sealed record ReviewProcessListItemDto(
    Guid Id,
    Guid AcademicLoadProposalId,
    Guid DivisionHeadId,
    int Version,
    DateTime? SubmittedAtUtc,
    DateTime CreatedAtUtc);

public sealed record ReviewProcessDetailDto(
    Guid Id,
    int Status,
    Guid AcademicLoadProposalId,
    Guid DivisionHeadId,
    int Version,
    DateTime? SubmittedAtUtc,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    Guid CorrelationId,
    IEnumerable<ObservationDetailDto> Observations);

public sealed record ObservationDetailDto(
    Guid Id,
    int TargetType,
    Guid TargetId,
    string Description,
    DateTime CreatedAtUtc);

public sealed record ReturnRequestDto(
    IEnumerable<ObservationInputDto> Observations);

public sealed record ObservationInputDto(
    int TargetType,
    Guid TargetId,
    string Description);