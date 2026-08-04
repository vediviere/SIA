
using SIA.AcademicService.Application.Interfaces.Queries;
using SIA.AcademicService.Contracts.Responses.AcademicPeriods;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Application.UseCases.AcademicPeriods;

public sealed class GetAllAcademicPeriodsUseCase
{
    private readonly IAcademicPeriodsQueries _queries;

    public GetAllAcademicPeriodsUseCase(IAcademicPeriodsQueries queries)
    {
        _queries = queries;
    }

    public async Task<List<AcademicPeriodResponse>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var academicPeriods = await _queries.GetAllAsync(cancellationToken);

        return academicPeriods.Select(academicPeriod => new AcademicPeriodResponse
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
