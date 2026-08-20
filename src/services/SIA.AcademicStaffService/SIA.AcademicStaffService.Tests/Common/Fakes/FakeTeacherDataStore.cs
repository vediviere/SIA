using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.Professors;
using SIA.AcademicStaffService.Domain.Entities;

namespace SIA.AcademicStaffService.Tests.Common.Fakes;

public sealed class FakeTeacherDataStore : ITeacherDataStore
{
    public Teacher? TeacherById { get; set; }
    public bool PersonAlreadyProfessorResult { get; set; }

    public bool TeacherAdded { get; private set; }
    public bool TeacherUpdated { get; private set; }
    public bool TeacherActivated { get; private set; }
    public bool TeacherDeactivated { get; private set; }

    public Task<bool> PersonAlreadyProfessorAsync(Guid tenantId, Guid personId, CancellationToken cancellationToken)
        => Task.FromResult(PersonAlreadyProfessorResult);

    public Task<Teacher?> GetProfessorByIdAsync(Guid tenantId, Guid professorId, CancellationToken cancellationToken)
        => Task.FromResult(TeacherById);

    public Task AddProfessorWithOutboxAsync(Teacher teacher, TeacherCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        TeacherAdded = true;
        return Task.CompletedTask;
    }

    public Task UpdateProfessorWithOutboxAsync(Teacher teacher, TeacherUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        TeacherUpdated = true;
        return Task.CompletedTask;
    }

    public Task ActivateProfessorWithOutboxAsync(Teacher teacher, TeacherActivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        TeacherActivated = true;
        return Task.CompletedTask;
    }

    public Task DeactivateProfessorWithOutboxAsync(Teacher teacher, TeacherDeactivatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        TeacherDeactivated = true;
        return Task.CompletedTask;
    }
}