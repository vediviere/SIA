using Moq;
using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Application.UseCases.Subjects;
using SIA.AcademicService.Contracts.IntegrationEvents.Subjects;
using SIA.AcademicService.Contracts.Requests.Subjects;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Tests.Application.UseCases.Subjects;

public class UpdateSubjectUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldUpdateSubjectAndReturnResponse()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var subject = new Subject(tenantId, "MAT-001", "Matemáticas", 1, 4, 2, 6);

        var request = new UpdateSubjectRequest
        {
            Code = "MAT-002",
            Name = "Cálculo",
            Semester = 2,
            TheoryHours = 5,
            PracticeHours = 3,
            Credits = 8
        };

        var dataStoreMock = new Mock<ISubjectDataStore>();

        dataStoreMock
            .Setup(x => x.GetSubjectByIdAsync(
                tenantId,
                subject.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(subject);

        dataStoreMock
            .Setup(x => x.SubjectCodeExistsAsync(
                tenantId,
                "MAT-002",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        dataStoreMock
            .Setup(x => x.UpdateSubjectWithOutboxAsync(
                It.IsAny<Subject>(),
                It.IsAny<SubjectUpdatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var useCase = new UpdateSubjectUseCase(dataStoreMock.Object);

        // Act
        var response = await useCase.ExecuteAsync(tenantId,subject.Id,request,correlationId,CancellationToken.None);

        // Assert
        Assert.Equal(subject.Id, response.Id);
        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal("MAT-002", response.Code);
        Assert.Equal("Cálculo", response.Name);
        Assert.Equal(2, response.Semester);
        Assert.Equal(5, response.TheoryHours);
        Assert.Equal(3, response.PracticeHours);
        Assert.Equal(8, response.Credits);
        Assert.True(response.Status);
        Assert.NotNull(response.UpdatedAtUtc);
        Assert.Equal(correlationId, response.CorrelationId);
    }


    [Fact]
    public async Task ExecuteAsync_WithNonExistingSubject_ShouldThrowSubjectNotFoundException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var request = new UpdateSubjectRequest
        {
            Code = "MAT-002",
            Name = "Cálculo",
            Semester = 2,
            TheoryHours = 5,
            PracticeHours = 3,
            Credits = 8
        };

        var dataStoreMock = new Mock<ISubjectDataStore>();

        dataStoreMock
            .Setup(x => x.GetSubjectByIdAsync(
                tenantId,
                subjectId,
                It.IsAny<CancellationToken>())).ReturnsAsync((Subject?)null);

        var useCase = new UpdateSubjectUseCase(dataStoreMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<SubjectNotFoundException>(() =>
            useCase.ExecuteAsync(tenantId,subjectId,request,correlationId,CancellationToken.None)
        );

        dataStoreMock.Verify(
            x => x.UpdateSubjectWithOutboxAsync(
                It.IsAny<Subject>(),
                It.IsAny<SubjectUpdatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }


    [Fact]
    public async Task ExecuteAsync_WithDuplicateCode_ShouldThrowDuplicateSubjectCodeException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var subject = new Subject(tenantId, "MAT-001", "Matemáticas", 1, 4, 2, 6);

        var request = new UpdateSubjectRequest
        {
            Code = "MAT-002",
            Name = "Cálculo",
            Semester = 2,
            TheoryHours = 5,
            PracticeHours = 3,
            Credits = 8
        };

        var dataStoreMock = new Mock<ISubjectDataStore>();

        dataStoreMock
            .Setup(x => x.GetSubjectByIdAsync(
                tenantId,
                subject.Id,
                It.IsAny<CancellationToken>())).ReturnsAsync(subject);

        dataStoreMock
            .Setup(x => x.SubjectCodeExistsAsync(
                tenantId,
                "MAT-002",
                It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var useCase = new UpdateSubjectUseCase(dataStoreMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateSubjectCodeException>(() =>
            useCase.ExecuteAsync(tenantId,subject.Id,request,correlationId,CancellationToken.None)
        );

        dataStoreMock.Verify(
            x => x.UpdateSubjectWithOutboxAsync(
                It.IsAny<Subject>(),
                It.IsAny<SubjectUpdatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }


    [Fact]
    public async Task ExecuteAsync_WithSameCode_ShouldNotCheckIfCodeExists()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var subject = new Subject(tenantId, "MAT-001", "Matemáticas", 1, 4, 2, 6);

        var request = new UpdateSubjectRequest
        {
            Code = " MAT-001 ",
            Name = "Cálculo",
            Semester = 2,
            TheoryHours = 5,
            PracticeHours = 3,
            Credits = 8
        };

        var dataStoreMock = new Mock<ISubjectDataStore>();

        dataStoreMock
            .Setup(x => x.GetSubjectByIdAsync(
                tenantId,
                subject.Id,
                It.IsAny<CancellationToken>())).ReturnsAsync(subject);

        dataStoreMock
            .Setup(x => x.UpdateSubjectWithOutboxAsync(
                It.IsAny<Subject>(),
                It.IsAny<SubjectUpdatedIntegrationEvent>(),
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var useCase = new UpdateSubjectUseCase(dataStoreMock.Object);

        // Act
        var response = await useCase.ExecuteAsync(tenantId,subject.Id,request,correlationId,CancellationToken.None);

        // Assert
        Assert.Equal("MAT-001", response.Code);
        Assert.Equal("Cálculo", response.Name);

        dataStoreMock.Verify(
            x => x.SubjectCodeExistsAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }


    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldCreateCorrectIntegrationEvent()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var subject = new Subject(tenantId,"MAT-001","Matemáticas",1,4,2,6);

        var request = new UpdateSubjectRequest
        {
            Code = "MAT-002",
            Name = "Cálculo",
            Semester = 2,
            TheoryHours = 5,
            PracticeHours = 3,
            Credits = 8
        };

        var dataStoreMock = new Mock<ISubjectDataStore>();

        dataStoreMock
            .Setup(x => x.GetSubjectByIdAsync(
                tenantId,
                subject.Id,
                It.IsAny<CancellationToken>())).ReturnsAsync(subject);

        dataStoreMock
            .Setup(x => x.SubjectCodeExistsAsync(
                tenantId,
                "MAT-002",
                It.IsAny<CancellationToken>())).ReturnsAsync(false);

        SubjectUpdatedIntegrationEvent? capturedEvent = null;

        dataStoreMock
            .Setup(x => x.UpdateSubjectWithOutboxAsync(
                It.IsAny<Subject>(),
                It.IsAny<SubjectUpdatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()))
            .Callback<Subject, SubjectUpdatedIntegrationEvent, CancellationToken>(
                (_, integrationEvent, _) =>
                {
                    capturedEvent = integrationEvent;
                })
            .Returns(Task.CompletedTask);

        var useCase = new UpdateSubjectUseCase(dataStoreMock.Object);

        // Act
        await useCase.ExecuteAsync(tenantId,subject.Id,request,correlationId,CancellationToken.None);

        // Assert
        Assert.NotNull(capturedEvent);

        Assert.NotEqual(Guid.Empty, capturedEvent.EventId);
        Assert.Equal(correlationId, capturedEvent.CorrelationId);
        Assert.Equal(tenantId, capturedEvent.TenantId);
        Assert.Equal(subject.Id, capturedEvent.SubjectId);
        Assert.Equal("MAT-002", capturedEvent.Code);
        Assert.Equal("Cálculo", capturedEvent.Name);
        Assert.Equal(2, capturedEvent.Semester);
        Assert.Equal(5, capturedEvent.TheoryHours);
        Assert.Equal(3, capturedEvent.PracticeHours);
        Assert.Equal(8, capturedEvent.Credits);
        Assert.True(capturedEvent.Status);
        Assert.Equal(1, capturedEvent.Version);
        Assert.NotEqual(default, capturedEvent.OccurredAtUtc);
    }
}