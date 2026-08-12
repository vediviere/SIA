using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Contracts.Requests.Building;

public sealed record UpdateBuildingRequest
{
    public required string Code { get; init; }

    public required string Name { get; init; }

    public string? Description { get; init; }
}