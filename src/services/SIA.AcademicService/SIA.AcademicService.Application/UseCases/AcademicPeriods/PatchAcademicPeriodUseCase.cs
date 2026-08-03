
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Contracts.IntegrationEvents.AcademicPeriods;
using SIA.AcademicService.Contracts.Requests.AcademicPeriods;
using SIA.AcademicService.Contracts.Responses.AcademicPeriods;

namespace SIA.AcademicService.Application.UseCases.AcademicPeriods;

public sealed class PatchAcademicPeriodUseCase
{
    private readonly IAcademicPeriodsDataStore _dataStore;

    public PatchAcademicPeriodUseCase(IAcademicPeriodsDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<PatchAcademicPeriodResponse> ExecuteAsync(Guid id, PatchAcademicPeriodRequest request, Guid correlationId, CancellationToken cancellationToken)
    {
        var academicPeriod = await _dataStore.GetByIdAsync(id, cancellationToken);

        if (academicPeriod is null)
        {
            throw new InvalidOperationException($"No existe un periodo académico con el id {id}.");
        }

        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        if (normalizedCode != academicPeriod.Code)
        {
            var codeExists = await _dataStore.AcademicPeriodCodeExistsAsync(academicPeriod.TenantId, normalizedCode, cancellationToken);

            if (codeExists)
            {
                throw new InvalidOperationException($"Ya existe un periodo académico con el código {normalizedCode}.");
            }
        }

        academicPeriod.Update(
            normalizedCode,
            request.Name,
            request.StartDate,
            request.EndDate,
            request.AcademicLoadProcessStartDate,
            request.AcademicLoadProcessEndDate,
            request.EnrollmentProcessStartDate,
            request.EnrollmentProcessEndDate,
            request.PlanningSubmissionDate,
            request.FirstPartialGradeReportDate,
            request.SecondPartialGradeReportDate,
            request.ThirdPartialGradeReportDate,
            request.FinalMinutesSubmissionDate);

        var integrationEvent = new AcademicPeriodUpdatedIntegrationEvent
            {
                EventId = Guid.NewGuid(),
                CorrelationId = correlationId,
                OccurredAtUtc = academicPeriod.UpdatedAtUtc!.Value,
                TenantId = academicPeriod.TenantId,
                AcademicPeriodId = academicPeriod.Id,
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
                Version = 1
            };

        await _dataStore.UpdateAcademicPeriodWithOutboxAsync(academicPeriod, integrationEvent, cancellationToken);

        return new PatchAcademicPeriodResponse
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
            UpdatedAtUtc = academicPeriod.UpdatedAtUtc,
            CorrelationId = correlationId
        };
    }

}