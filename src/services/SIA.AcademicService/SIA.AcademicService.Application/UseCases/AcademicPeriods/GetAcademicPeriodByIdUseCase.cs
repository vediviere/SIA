using SIA.AcademicService.Application.Interfaces.Queries;
using SIA.AcademicService.Contracts.Responses;
using SIA.AcademicService.Contracts.Responses.AcademicPeriods;

namespace SIA.AcademicService.Application.UseCases.AcademicPeriods;

public sealed class GetAcademicPeriodByIdUseCase
{
    private readonly IAcademicPeriodsQueries _queries;

    public GetAcademicPeriodByIdUseCase(IAcademicPeriodsQueries queries)
    {
        _queries = queries;
    }

    public async Task<AcademicPeriodResponse> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var academicPeriod = await _queries.GetByIdAsync(id, cancellationToken);

        if (academicPeriod is null)
        {
            throw new InvalidOperationException($"No existe un periodo académico con el id {id}.");
        }

        return new AcademicPeriodResponse
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