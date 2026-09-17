namespace SIA.AdminBff.Clients.Workflow;

public sealed record ReviewProcessDto(
    Guid Id,
    Guid TenantId,
    string Status,
    DateTime CreatedAtUtc
);