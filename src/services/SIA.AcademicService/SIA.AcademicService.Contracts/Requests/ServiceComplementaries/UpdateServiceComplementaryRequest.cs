using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.AcademicService.Contracts.Requests.ServiceComplementaries;

public sealed class UpdateServiceComplementaryRequest
{
    public required bool Type { get; init; }
    public required int Credit { get; init; }
}