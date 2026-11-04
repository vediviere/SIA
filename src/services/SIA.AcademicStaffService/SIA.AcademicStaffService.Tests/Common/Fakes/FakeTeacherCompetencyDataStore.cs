using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.TeacherCompetency;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Tests.Common.Fakes;

public sealed class FakeTeacherCompetencyDataStore : ITeacherCompetencyDataStore
{
    public TeacherCompetency? TeacherCompetencyById { get; set; }
    public bool CompetencyAlreadyExistsResult { get; set; }

    public TeacherCompetency? AddedTeacherCompetency { get; private set; }
    public TeacherCompetencyCreatedIntegrationEvent? AddedEvent { get; private set; }

    public TeacherCompetency? UpdatedTeacherCompetency { get; private set; }
    public TeacherCompetencyUpdatedIntegrationEvent? UpdatedEvent { get; private set; }

    public TeacherCompetency? ActivatedTeacherCompetency { get; private set; }
    public TeacherCompetencyActivatedIntegrationEvent? ActivatedEvent { get; private set; }

    public TeacherCompetency? DeactivatedTeacherCompetency { get; private set; }
    public TeacherCompetencyDeactivatedIntegrationEvent? DeactivatedEvent { get; private set; }

    public Task<bool> CompetencyAlreadyExistsForTeacherAsync(Guid tenantId, Guid teacherId, Guid competencyId, CancellationToken cancellationToken)
        => Task.FromResult(CompetencyAlreadyExistsResult);

    public Task<TeacherCompetency?> GetTeacherCompetencyByIdAsync(Guid tenantId, Guid teacherCompetencyId, CancellationToken cancellationToken)
        => Task.FromResult(TeacherCompetencyById);

    public Task AddTeacherCompetencyWithOutboxAsync(TeacherCompetency teacherCompetency, TeacherCompetencyCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        AddedTeacherCompetency = teacherCompetency;
        AddedEvent = integrationEvent;
        return Task.CompletedTask;
    }

    public Task UpdateTeacherCompetencyWithOutboxAsync(TeacherCompetency teacherCompetency, TeacherCompetencyUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        UpdatedTeacherCompetency = teacherCompetency;
        UpdatedEvent = integrationEvent;
        return Task.CompletedTask;
    }

    public Task ActivateTeacherCompetencyWithOutboxAsync(TeacherCompetency teacherCompetency, TeacherCompetencyActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        ActivatedTeacherCompetency = teacherCompetency;
        ActivatedEvent = integrationEvent;
        return Task.CompletedTask;
    }

    public Task DeactivateTeacherCompetencyWithOutboxAsync(TeacherCompetency teacherCompetency, TeacherCompetencyDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        DeactivatedTeacherCompetency = teacherCompetency;
        DeactivatedEvent = integrationEvent;
        return Task.CompletedTask;
    }
}