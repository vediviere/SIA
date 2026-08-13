
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoad;
using SIA.SchedulingService.Contracts.Requests.AcademicLoad;
using SIA.SchedulingService.Contracts.Responses.AcademicLoad;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Application.UseCases.AcademicLoads;

public sealed class CreateAcademicLoadUseCase
{
    private readonly IAcademicLoadDataStore _dataStore;

    public CreateAcademicLoadUseCase(IAcademicLoadDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<CreateAcademicLoadResponse> ExecuteAsync(CreateAcademicLoadRequest request, Guid correlationId, CancellationToken cancellationToken)
    {
        var academicLoad = new AcademicLoad(
            request.TenantId,
            request.TeacherId,
            request.DivisionId,
            request.AcademicPeriodId,
            request.OfficialLetterNumber,
            request.ProposedDate,
            request.ClassHours,
            request.SupportHours,
            request.AssignmentDate);

        var integrationEvent = new AcademicLoadCreatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = academicLoad.CreatedAtUtc,
            TenantId = academicLoad.TenantId,
            AcademicLoadId = academicLoad.Id,
            TeacherId = academicLoad.TeacherId,
            DivisionId = academicLoad.DivisionId,
            AcademicPeriodId = academicLoad.AcademicPeriodId,
            OfficialLetterNumber = academicLoad.OfficialLetterNumber,
            ProposedDate = academicLoad.ProposedDate,
            AssignmentDate = academicLoad.AssignmentDate,
            ClassHours = academicLoad.ClassHours,
            SupportHours = academicLoad.SupportHours,
            Status = academicLoad.Status,
            Version = 1
        };

        await _dataStore.AddAcademicLoadWithOutboxAsync(academicLoad, integrationEvent, cancellationToken);

        return new CreateAcademicLoadResponse
        {
            Id = academicLoad.Id,
            TenantId = academicLoad.TenantId,
            TeacherId = academicLoad.TeacherId,
            DivisionId = academicLoad.DivisionId,
            AcademicPeriodId = academicLoad.AcademicPeriodId,
            OfficialLetterNumber = academicLoad.OfficialLetterNumber,
            ProposedDate = academicLoad.ProposedDate,
            ClassHours = academicLoad.ClassHours,
            SupportHours = academicLoad.SupportHours,
            AssignmentDate = academicLoad.AssignmentDate,
            Status = academicLoad.Status,
            CreatedAtUtc = academicLoad.CreatedAtUtc,
            CorrelationId = correlationId
        };
    }

}