
using SIA.AcademicService.Application.DTOs.AcademicPeriod;
using SIA.AcademicService.Application.Interfaces.Queries;
using SIA.AcademicService.Contracts.Responses.AcademicPeriods;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Application.UseCases.AcademicPeriods;

public sealed class SearchAcademicPeriodsUseCase
{
    private readonly IAcademicPeriodQueries _queries;

    public SearchAcademicPeriodsUseCase(IAcademicPeriodQueries queries)
    {
        _queries = queries;
    }

    public async Task<IReadOnlyCollection<AcademicPeriodDto>> ExecuteAsync(AcademicPeriodFilter filter, CancellationToken cancellationToken)
    {
        var academicPeriods = await _queries.SearchAsync(filter, cancellationToken);

        return academicPeriods.Select(academicPeriod => new AcademicPeriodDto
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
        }).ToList();
    }

}
