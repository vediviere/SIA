using Moq;
using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Application.UseCases.Subjects;
using SIA.AcademicService.Contracts.IntegrationEvents.Subjects;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Tests.Application.UseCases.Subjects;

public class RestoreSubjectUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidSubject_ShouldRestoreSubject()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var subject = new Subject(tenantId, "MAT-001", "Matemáticas", 1, 4, 2, 6);

        subject.SoftDelete();

        var dataStoreMock = new Mock<ISubjectDataStore>();

        dataStoreMock.Setup(x => x.GetSubjectByIdAsync(
                tenantId,
                subject.Id,
                It.IsAny<CancellationToken>())).ReturnsAsync(subject);

        dataStoreMock.Setup(x => x.RestoreSubjectWithOutboxAsync(
                It.IsAny<Subject>(),
                It.IsAny<SubjectRestoredIntegrationEvent>(),
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var useCase = new RestoreSubjectUseCase(dataStoreMock.Object);

        // Act
        await useCase.ExecuteAsync(tenantId, subject.Id, correlationId, CancellationToken.None);

        // Assert
        Assert.True(subject.Status);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistingSubject_ShouldThrowSubjectNotFoundException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var dataStoreMock = new Mock<ISubjectDataStore>();

        dataStoreMock
            .Setup(x => x.GetSubjectByIdAsync(
                tenantId,
                subjectId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Subject?)null);

        var useCase = new RestoreSubjectUseCase(dataStoreMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<SubjectNotFoundException>(() =>
            useCase.ExecuteAsync(tenantId,subjectId, correlationId,CancellationToken.None)
        );

        // Verify
        dataStoreMock.Verify(
            x => x.RestoreSubjectWithOutboxAsync(
                It.IsAny<Subject>(),
                It.IsAny<SubjectRestoredIntegrationEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidSubject_ShouldCreateCorrectIntegrationEvent()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var subject = new Subject(tenantId, "MAT-001", "Matemáticas", 1, 4, 2, 6);

        subject.SoftDelete();

        var dataStoreMock = new Mock<ISubjectDataStore>();

        dataStoreMock
            .Setup(x => x.GetSubjectByIdAsync(
                tenantId,
                subject.Id,
                It.IsAny<CancellationToken>())).ReturnsAsync(subject);

        SubjectRestoredIntegrationEvent? capturedEvent = null;

        dataStoreMock.Setup(x => x.RestoreSubjectWithOutboxAsync(
                It.IsAny<Subject>(),
                It.IsAny<SubjectRestoredIntegrationEvent>(),
                It.IsAny<CancellationToken>()))
            .Callback<Subject, SubjectRestoredIntegrationEvent, CancellationToken>(
                (_, integrationEvent, _) =>
                {
                    capturedEvent = integrationEvent;
                })
            .Returns(Task.CompletedTask);

        var useCase = new RestoreSubjectUseCase(dataStoreMock.Object);

        // Act
        await useCase.ExecuteAsync(tenantId,subject.Id,correlationId,CancellationToken.None);

        // Assert
        Assert.NotNull(capturedEvent);

        Assert.NotEqual(Guid.Empty, capturedEvent.EventId);
        Assert.Equal(correlationId, capturedEvent.CorrelationId);
        Assert.Equal(tenantId, capturedEvent.TenantId);
        Assert.Equal(subject.Id, capturedEvent.SubjectId);
        Assert.Equal(1, capturedEvent.Version);
        Assert.NotEqual(default, capturedEvent.OccurredAtUtc);
    }
}
