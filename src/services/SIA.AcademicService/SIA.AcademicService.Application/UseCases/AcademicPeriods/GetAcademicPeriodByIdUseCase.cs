using SIA.AcademicService.Application.DTOs.AcademicPeriod;
using SIA.AcademicService.Application.Interfaces.Queries;
using SIA.AcademicService.Contracts.Responses;
using SIA.AcademicService.Contracts.Responses.AcademicPeriods;

namespace SIA.AcademicService.Application.UseCases.AcademicPeriods;

public sealed class GetAcademicPeriodByIdUseCase
{
    private readonly IAcademicPeriodQueries _queries;

    public GetAcademicPeriodByIdUseCase(IAcademicPeriodQueries queries)
    {
        _queries = queries;
    }

    public async Task<AcademicPeriodDto> ExecuteAsync(Guid tenantId, Guid academicPeriodId, CancellationToken cancellationToken)
    {
        var academicPeriod = await _queries.GetByIdAsync(tenantId, academicPeriodId, cancellationToken);

        if (academicPeriod is null)
        {
            throw new InvalidOperationException($"No existe un periodo académico con el id {academicPeriodId}.");
        }

        return new AcademicPeriodDto
        {
            Id = academicPeriod.Id,
            TenantId = academicPeriod.TenantId,
            Code = academicPeriod.Code,
            Name = academicPeriod.Name,
            StartDate = academicPeriod.StartDate,
            EndDate = academicPeriod.EndDate,
            AcademicLoadProcessStartDate = academicPeriod.AcademicLoadProcessStartDate,
            AcademicLoadProcessEndDate = academicPeriod.AcademicLoadProcessEndDate,
            EnrollmentProcessStartDate = academicPeriod.EnrollmentProcessStartDate,
            EnrollmentProcessEndDate = academicPeriod.EnrollmentProcessEndDate,
            PlanningSubmissionDate = academicPeriod.PlanningSubmissionDate,
            FirstPartialGradeReportDate = academicPeriod.FirstPartialGradeReportDate,
            SecondPartialGradeReportDate = academicPeriod.SecondPartialGradeReportDate,
            ThirdPartialGradeReportDate = academicPeriod.ThirdPartialGradeReportDate,
            FinalMinutesSubmissionDate = academicPeriod.FinalMinutesSubmissionDate,
            Status = academicPeriod.Status,
            CreatedAtUtc = academicPeriod.CreatedAtUtc,
            UpdatedAtUtc = academicPeriod.UpdatedAtUtc
        };
    }
}