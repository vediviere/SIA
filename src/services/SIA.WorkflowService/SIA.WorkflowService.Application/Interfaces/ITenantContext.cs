using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.WorkflowService.Application.Interfaces
{
    public interface ITenantContext
    {
        Guid TenantId { get; }
    }
}
