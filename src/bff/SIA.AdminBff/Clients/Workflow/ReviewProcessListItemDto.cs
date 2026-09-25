namespace SIA.AdminBff.Clients.Workflow;

public sealed record ReviewProcessListItemDto(
    Guid Id,
    string Status,
    DateTime CreatedAtUtc);

public sealed record ReviewProcessDetailDto(
    Guid Id,
    string Status,
    DateTime CreatedAtUtc);

public sealed record ReturnRequestDto(
    IEnumerable<ObservationInputDto> Observations);

public sealed record ObservationInputDto(
    int TargetType,
    Guid TargetId,
    string Description);