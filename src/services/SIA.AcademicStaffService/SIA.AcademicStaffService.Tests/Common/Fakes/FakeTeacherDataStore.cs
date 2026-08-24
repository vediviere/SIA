using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.Professors;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Tests.Common.Fakes;

public sealed class FakeTeacherDataStore : ITeacherDataStore
{
    public Teacher? TeacherById { get; set; }
    public bool PersonAlreadyProfessorResult { get; set; }

    public Teacher? AddedTeacher { get; private set; }
    public TeacherCreatedIntegrationEvent? AddedEvent { get; private set; }

    public Teacher? UpdatedTeacher { get; private set; }
    public TeacherUpdatedIntegrationEvent? UpdatedEvent { get; private set; }

    public Teacher? ActivatedTeacher { get; private set; }
    public TeacherActivatedIntegrationEvent? ActivatedEvent { get; private set; }

    public Teacher? DeactivatedTeacher { get; private set; }
    public TeacherDeactivatedIntegrationEvent? DeactivatedEvent { get; private set; }


    public Task<bool> PersonAlreadyProfessorAsync(Guid tenantId, Guid personId, CancellationToken cancellationToken)
        => Task.FromResult(PersonAlreadyProfessorResult);

    public Task<Teacher?> GetProfessorByIdAsync(Guid tenantId, Guid professorId, CancellationToken cancellationToken)
        => Task.FromResult(TeacherById);

    public Task AddProfessorWithOutboxAsync(Teacher teacher, TeacherCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        AddedTeacher = teacher;
        AddedEvent = integrationEvent;
        return Task.CompletedTask;
    }

    public Task UpdateProfessorWithOutboxAsync(Teacher teacher, TeacherUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        UpdatedTeacher = teacher;
        UpdatedEvent = integrationEvent;
        return Task.CompletedTask;
    }

    public Task ActivateProfessorWithOutboxAsync(Teacher teacher, TeacherActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        ActivatedTeacher = teacher;
        ActivatedEvent = integrationEvent;
        return Task.CompletedTask;
    }

    public Task DeactivateProfessorWithOutboxAsync(Teacher teacher, TeacherDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        DeactivatedTeacher = teacher;
        DeactivatedEvent = integrationEvent;
        return Task.CompletedTask;
    }
}