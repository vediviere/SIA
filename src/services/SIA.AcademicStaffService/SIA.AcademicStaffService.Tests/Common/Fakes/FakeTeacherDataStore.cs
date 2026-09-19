using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.Teacher;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Tests.Common.Fakes;

public sealed class FakeTeacherDataStore : ITeacherDataStore
{
    public Teacher? TeacherById { get; set; }
    public bool PersonAlreadyTeacherResult { get; set; }

    public Teacher? AddedTeacher { get; private set; }
    public TeacherCreatedIntegrationEvent? AddedEvent { get; private set; }

    public Teacher? UpdatedTeacher { get; private set; }
    public TeacherUpdatedIntegrationEvent? UpdatedEvent { get; private set; }

    public Teacher? ActivatedTeacher { get; private set; }
    public TeacherActivatedIntegrationEvent? ActivatedEvent { get; private set; }

    public Teacher? DeactivatedTeacher { get; private set; }
    public TeacherDeactivatedIntegrationEvent? DeactivatedEvent { get; private set; }

    public Task<bool> PersonAlreadyTeacherAsync(Guid tenantId, Guid personId, CancellationToken cancellationToken)
        => Task.FromResult(PersonAlreadyTeacherResult);

    public Task<Teacher?> GetTeacherByIdAsync(Guid tenantId, Guid teacherId, CancellationToken cancellationToken)
        => Task.FromResult(TeacherById);

    public Task AddTeacherWithOutboxAsync(Teacher teacher, TeacherCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        AddedTeacher = teacher;
        AddedEvent = integrationEvent;
        return Task.CompletedTask;
    }

    public Task UpdateTeacherWithOutboxAsync(Teacher teacher, TeacherUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        UpdatedTeacher = teacher;
        UpdatedEvent = integrationEvent;
        return Task.CompletedTask;
    }

    public Task ActivateTeacherWithOutboxAsync(Teacher teacher, TeacherActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        ActivatedTeacher = teacher;
        ActivatedEvent = integrationEvent;
        return Task.CompletedTask;
    }

    public Task DeactivateTeacherWithOutboxAsync(Teacher teacher, TeacherDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        DeactivatedTeacher = teacher;
        DeactivatedEvent = integrationEvent;
        return Task.CompletedTask;
    }
}