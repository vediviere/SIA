using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.Common.Exceptions.AcademicOffering;
using SIA.SchedulingService.Application.DTOs.AcademicOffering;
using SIA.SchedulingService.Application.Interfaces.Queries;

namespace SIA.SchedulingService.Application.UseCases.AcademicOfferings;

public sealed class GetAcademicOfferingByIdUseCase
{
    private readonly IAcademicOfferingQueries _queries;

    public GetAcademicOfferingByIdUseCase(IAcademicOfferingQueries queries)
    {
        _queries = queries;
    }

    public async Task<AcademicOfferingDto> ExecuteAsync(Guid tenantId, Guid offeringId, CancellationToken cancellationToken)
    {
        var academicOffering = await _queries.GetByIdAsync(tenantId, offeringId, cancellationToken);

        if (academicOffering is null)
        {
            throw new AcademicOfferingNotFoundException(offeringId);
        }

        return new AcademicOfferingDto
        {
            Id = academicOffering.Id,
            TenantId = academicOffering.TenantId,
            GroupId = academicOffering.GroupId,
            SubjectId = academicOffering.SubjectId,
            AcademicLoadId = academicOffering.AcademicLoadId,
            OfferingStatus = academicOffering.OfferingStatus,
            Status = academicOffering.Status,
            CreatedAtUtc = academicOffering.CreatedAtUtc,
            UpdatedAtUtc = academicOffering.UpdatedAtUtc
        };
    }
}