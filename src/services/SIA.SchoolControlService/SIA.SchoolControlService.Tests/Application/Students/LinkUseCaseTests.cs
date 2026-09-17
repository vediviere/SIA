using Microsoft.EntityFrameworkCore;
using SIA.SchoolControlService.Application.Common.Exceptions.Students;
using SIA.SchoolControlService.Application.UseCases.Students;
using SIA.SchoolControlService.Contracts.Requests.Students;
using SIA.SchoolControlService.Domain.Entities;
using SIA.SchoolControlService.Infrastructure.Persistence.Contexts;
using SIA.SchoolControlService.Infrastructure.Persistence.DataStores;

namespace SIA.SchoolControlService.Tests.Application.Students;

public sealed class LinkUseCaseTests
{
  [Fact]
  public async Task ExecuteAsync_WithValidStudent_ShouldLinkUser()
  {
    await using var dbContext = CreateDbContext();
    var tenantId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    var student = await AddStudentAsync(dbContext, tenantId);
    var useCase = CreateUseCase(dbContext);

    var response = await useCase.ExecuteAsync(CreateRequest(tenantId, userId), CancellationToken.None);

    Assert.True(response.Linked);
    Assert.Equal(student.Id, response.StudentId);
    Assert.Equal(userId, response.UserId);
    Assert.Equal("2026-AB-001", response.StudentNumber);

    var storedStudent = await dbContext.Students.SingleAsync();
    Assert.Equal(userId, storedStudent.UserId);
  }

  [Fact]
  public async Task ExecuteAsync_WhenStudentDoesNotExist_ShouldThrowNotFound()
  {
    await using var dbContext = CreateDbContext();
    var useCase = CreateUseCase(dbContext);

    var action = () => useCase.ExecuteAsync(
      CreateRequest(Guid.NewGuid(), Guid.NewGuid()),
      CancellationToken.None);

    await Assert.ThrowsAsync<StudentNotFoundException>(action);
  }

  [Fact]
  public async Task ExecuteAsync_WhenStudentIsInactive_ShouldThrowConflict()
  {
    await using var dbContext = CreateDbContext();
    var tenantId = Guid.NewGuid();
    var student = await AddStudentAsync(dbContext, tenantId);

    dbContext.Entry(student).Property(value => value.Status).CurrentValue = false;
    await dbContext.SaveChangesAsync();

    var useCase = CreateUseCase(dbContext);

    var action = () => useCase.ExecuteAsync(
      CreateRequest(tenantId, Guid.NewGuid()),
      CancellationToken.None);

    await Assert.ThrowsAsync<InactiveException>(action);
    Assert.Null(student.UserId);
  }

  [Fact]
  public async Task ExecuteAsync_WithSameUser_ShouldBeIdempotent()
  {
    await using var dbContext = CreateDbContext();
    var tenantId = Guid.NewGuid();
    var userId = Guid.NewGuid();
    await AddStudentAsync(dbContext, tenantId);

    var useCase = CreateUseCase(dbContext);
    var request = CreateRequest(tenantId, userId);

    var firstResponse = await useCase.ExecuteAsync(request, CancellationToken.None);
    var secondResponse = await useCase.ExecuteAsync(request, CancellationToken.None);

    Assert.True(firstResponse.Linked);
    Assert.True(secondResponse.Linked);
    Assert.Equal(firstResponse.StudentId, secondResponse.StudentId);
    Assert.Equal(userId, secondResponse.UserId);
    Assert.Single(dbContext.Students);
  }

  [Fact]
  public async Task ExecuteAsync_WithAnotherUser_ShouldThrowConflict()
  {
    await using var dbContext = CreateDbContext();
    var tenantId = Guid.NewGuid();
    await AddStudentAsync(dbContext, tenantId);

    var useCase = CreateUseCase(dbContext);

    await useCase.ExecuteAsync(
      CreateRequest(tenantId, Guid.NewGuid()),
      CancellationToken.None);

    var action = () => useCase.ExecuteAsync(
      CreateRequest(tenantId, Guid.NewGuid()),
      CancellationToken.None);

    await Assert.ThrowsAsync<LinkedException>(action);
  }

  [Fact]
  public async Task ExecuteAsync_WithAnotherTenant_ShouldThrowNotFound()
  {
    await using var dbContext = CreateDbContext();
    await AddStudentAsync(dbContext, Guid.NewGuid());

    var useCase = CreateUseCase(dbContext);

    var action = () => useCase.ExecuteAsync(
      CreateRequest(Guid.NewGuid(), Guid.NewGuid()),
      CancellationToken.None);

    await Assert.ThrowsAsync<StudentNotFoundException>(action);
  }

  private static LinkUseCase CreateUseCase(SchoolControlDbContext dbContext)
  {
    return new LinkUseCase(new StudentDataStore(dbContext));
  }

  private static LinkRequest CreateRequest(Guid tenantId, Guid userId)
  {
    return new LinkRequest
    {
      TenantId = tenantId,
      UserId = userId,
      StudentNumber = " 2026-ab-001 "
    };
  }

  private static async Task<Student> AddStudentAsync(SchoolControlDbContext dbContext, Guid tenantId)
  {
    var student = new Student(
      tenantId,
      "2026-AB-001",
      "Marco Antonio",
      "Morales",
      "Castillo");

    await dbContext.Students.AddAsync(student);
    await dbContext.SaveChangesAsync();

    return student;
  }

  private static SchoolControlDbContext CreateDbContext()
  {
    var options = new DbContextOptionsBuilder<SchoolControlDbContext>()
      .UseInMemoryDatabase(Guid.NewGuid().ToString())
      .Options;

    return new SchoolControlDbContext(options);
  }
}
