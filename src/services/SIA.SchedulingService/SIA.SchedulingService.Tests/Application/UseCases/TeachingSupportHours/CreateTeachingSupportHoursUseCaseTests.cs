using SIA.SchedulingService.Application.Common.Exceptions.TeachingSupportHours;
using SIA.SchedulingService.Application.UseCases.TeachingSupportHours;
using SIA.SchedulingService.Contracts.Requests.TeachingSupportHours;
using SIA.SchedulingService.Tests.Common.Fakes;

namespace SIA.SchedulingService.Tests.Application.UseCases.TeachingSupportHours;

public sealed class CreateTeachingSupportHoursUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldCreateTeachingSupportHours()
    {
        var tenantId = Guid.NewGuid();
        var activityId = Guid.NewGuid();
        var academicLoadId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var dataStore = new FakeTeachingSupportHoursDataStore();
        var useCase = new CreateTeachingSupportHoursUseCase(dataStore);

        var request = new CreateTeachingSupportHoursRequest
        {
            TenantId = tenantId,
            ActivityId = activityId,
            AcademicLoadId = academicLoadId,
            Hours = 5
        };
        var response = await useCase.ExecuteAsync(request, correlationId, CancellationToken.None);

        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal(activityId, response.ActivityId);
        Assert.Equal(academicLoadId, response.AcademicLoadId);
        Assert.Equal(5, response.Hours);
        Assert.True(response.Status);
        Assert.Equal(correlationId, response.CorrelationId);

        Assert.NotNull(dataStore.AddedTeachingSupportHours);
        Assert.Equal(tenantId, dataStore.AddedTeachingSupportHours.TenantId);
        Assert.Equal(5, dataStore.AddedTeachingSupportHours.Hours);

        Assert.NotNull(dataStore.AddedCreatedEvent);
        Assert.Equal(correlationId, dataStore.AddedCreatedEvent.CorrelationId);
        Assert.Equal(tenantId, dataStore.AddedCreatedEvent.TenantId);
        Assert.Equal(1, dataStore.AddedCreatedEvent.Version);
    }

    [Fact]
    public async Task ExecuteAsync_WhenAlreadyExists_ShouldThrowDuplicateTeachingSupportHoursException()
    {
        var dataStore = new FakeTeachingSupportHoursDataStore { ExistsResult = true };
        var useCase = new CreateTeachingSupportHoursUseCase(dataStore);

        var request = new CreateTeachingSupportHoursRequest
        {
            TenantId = Guid.NewGuid(),
            ActivityId = Guid.NewGuid(),
            AcademicLoadId = Guid.NewGuid(),
            Hours = 5
        };

        await Assert.ThrowsAsync<DuplicateTeachingSupportHoursException>(() => useCase.ExecuteAsync(request, Guid.NewGuid(), CancellationToken.None));

        Assert.Null(dataStore.AddedTeachingSupportHours);
        Assert.Null(dataStore.AddedCreatedEvent);
    }
}