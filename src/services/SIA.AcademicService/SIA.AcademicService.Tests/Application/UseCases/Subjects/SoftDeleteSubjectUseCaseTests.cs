using Moq;
using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Application.UseCases.Subjects;
using SIA.AcademicService.Contracts.IntegrationEvents.Subjects;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Tests.Application.UseCases.Subjects;

public class SoftDeleteSubjectUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidSubject_ShouldSoftDeleteSubject()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var subject = new Subject(tenantId, "MAT-001", "Matemáticas", 1, 4, 2, 6);

        var dataStoreMock = new Mock<ISubjectDataStore>();

        dataStoreMock
            .Setup(x => x.GetSubjectByIdAsync(
                tenantId,
                subject.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(subject);

        dataStoreMock
            .Setup(x => x.SoftDeleteSubjectWithOutboxAsync(
                It.IsAny<Subject>(),
                It.IsAny<SubjectDeletedIntegrationEvent>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var useCase = new SoftDeleteSubjectUseCase(dataStoreMock.Object);

        // Act
        await useCase.ExecuteAsync(tenantId,subject.Id,correlationId,CancellationToken.None);

        // Assert
        Assert.False(subject.Status);
        Assert.NotNull(subject.UpdatedAtUtc);
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

        var useCase = new SoftDeleteSubjectUseCase(dataStoreMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<SubjectNotFoundException>(() =>
            useCase.ExecuteAsync(tenantId,subjectId,correlationId,CancellationToken.None));

        // Verify
        dataStoreMock.Verify(
            x => x.SoftDeleteSubjectWithOutboxAsync(
                It.IsAny<Subject>(),
                It.IsAny<SubjectDeletedIntegrationEvent>(),
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

        var dataStoreMock = new Mock<ISubjectDataStore>();

        dataStoreMock
            .Setup(x => x.GetSubjectByIdAsync(
                tenantId,
                subject.Id,
                It.IsAny<CancellationToken>())).ReturnsAsync(subject);

        SubjectDeletedIntegrationEvent? capturedEvent = null;

        dataStoreMock
            .Setup(x => x.SoftDeleteSubjectWithOutboxAsync(
                It.IsAny<Subject>(),
                It.IsAny<SubjectDeletedIntegrationEvent>(),
                It.IsAny<CancellationToken>()))
            .Callback<Subject, SubjectDeletedIntegrationEvent, CancellationToken>(
                (_, integrationEvent, _) =>
                {
                    capturedEvent = integrationEvent;
                })
            .Returns(Task.CompletedTask);

        var useCase = new SoftDeleteSubjectUseCase(dataStoreMock.Object);

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

        dataStoreMock.Verify(
            x => x.SoftDeleteSubjectWithOutboxAsync(
                It.Is<Subject>(s =>
                    s.Id == subject.Id &&
                    !s.Status),
                It.Is<SubjectDeletedIntegrationEvent>(e =>
                    e.SubjectId == subject.Id &&
                    e.TenantId == tenantId &&
                    e.CorrelationId == correlationId &&
                    e.Version == 1),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}