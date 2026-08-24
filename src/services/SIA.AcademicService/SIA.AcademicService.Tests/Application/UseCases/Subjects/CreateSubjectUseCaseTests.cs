using Moq;
using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Application.UseCases.Subjects;
using SIA.AcademicService.Contracts.IntegrationEvents.Subjects;
using SIA.AcademicService.Contracts.Requests.Subjects;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Tests.Application.UseCases.Subjects;

public class CreateSubjectUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldCreatesubject()
    {
        //Arrange
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var request = new CreateSubjectRequest
        {
            TenantId = tenantId,
            Code = "MAT-001",
            Name = "Matemáticas",
            Semester = 1,
            TheoryHours = 4,
            PracticeHours = 2,
            Credits = 6
        };

        var dataStoreMok = new Mock<ISubjectDataStore>();

        dataStoreMok.Setup(x => x.SubjectCodeExistsAsync(
            tenantId,
            "MAT-001",
            It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var useCase = new CreateSubjectUseCase(dataStoreMok.Object);

        // Act
        var response = await useCase.ExecuteAsync(request, correlationId, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal(tenantId, response.TenantId);
        Assert.Equal("MAT-001", response.Code);
        Assert.Equal("Matemáticas", response.Name);
        Assert.Equal(1, response.Semester);
        Assert.Equal(4, response.TheoryHours);
        Assert.Equal(2, response.PracticeHours);
        Assert.Equal(6, response.Credits);

        dataStoreMok.Verify(
            x => x.AddSubjectWithOutboxAsync(
                It.IsAny<Subject>(),
                It.IsAny<SubjectCreatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithDuplicateCode_ShouldThrowDuplicateSubjectCodeException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var request = new CreateSubjectRequest
        {
            TenantId = tenantId,
            Code = "MAT-001",
            Name = "Matemáticas",
            Semester = 1,
            TheoryHours = 4,
            PracticeHours = 2,
            Credits = 6
        };

        var dataStoreMock = new Mock<ISubjectDataStore>();

        dataStoreMock.Setup(x => x.SubjectCodeExistsAsync(
            tenantId,
            "MAT-001",
            It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var useCase = new CreateSubjectUseCase(dataStoreMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateSubjectCodeException>(() =>
            useCase.ExecuteAsync(request,correlationId,CancellationToken.None)
        );

        dataStoreMock.Verify(
            x => x.AddSubjectWithOutboxAsync(
                It.IsAny<Subject>(),
                It.IsAny<SubjectCreatedIntegrationEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

    }
    
}
