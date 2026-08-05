using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Contracts.IntegrationEvents.AcademicPeriods;
using SIA.AcademicService.Contracts.Requests.AcademicPeriods;
using SIA.AcademicService.Contracts.Responses.AcademicPeriods;
using SIA.AcademicService.Domain.Entities;
using SIA.AcademicService.Application.Common.Exceptions;

namespace SIA.AcademicService.Application.UseCases.AcademicPeriods;

public sealed class CreateAcademicPeriodsUseCase
{
  private readonly IAcademicPeriodsDataStore _dataStore;

  public CreateAcademicPeriodsUseCase(IAcademicPeriodsDataStore dataStore)
  {
    _dataStore = dataStore;
  }

  public async Task<CreateAcademicPeriodResponse> ExecuteAsync(CreateAcademicPeriodRequest request, Guid correlationId, CancellationToken cancellationToken)
  {
    var normalizedCode = request.Code.Trim().ToUpperInvariant();

    var codeExists = await _dataStore.AcademicPeriodCodeExistsAsync(request.TenantId, normalizedCode, cancellationToken);

    if (codeExists)
    {
      throw new DuplicateAcademicPeriodCodeException(normalizedCode);
    }

    var academicPeriod = new AcademicPeriod(
            request.TenantId,
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

    var integrationEvent = new AcademicPeriodCreatedIntegrationEvent
    {
      EventId = Guid.NewGuid(),
      CorrelationId = correlationId,
      OccurredAtUtc = academicPeriod.CreatedAtUtc,
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

    await _dataStore.AddAcademicPeriodWithOutboxAsync(academicPeriod, integrationEvent, cancellationToken);

    return new CreateAcademicPeriodResponse
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
      CorrelationId = correlationId
    };
  }
}
