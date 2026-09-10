using Microsoft.EntityFrameworkCore;
using SIA.SchoolControlService.Application.UseCases.Students;
using SIA.SchoolControlService.Domain.Entities;
using SIA.SchoolControlService.Infrastructure.Persistence.Contexts;
using SIA.SchoolControlService.Infrastructure.Persistence.Queries;

namespace SIA.SchoolControlService.Tests.Application.Students;

public sealed class GetByNumberUseCaseTests
{
  [Fact]
  public async Task ExecuteAsync_WhenStudentExists_ShouldReturnStudent()
  {
    await using var dbContext = CreateDbContext();
    var tenantId = Guid.NewGuid();
    var student = new Student(tenantId, "2026-AB-001", "Marco", "Morales", "Castillo");

    await dbContext.Students.AddAsync(student);
    await dbContext.SaveChangesAsync();

    var useCase = new GetByNumberUseCase(new StudentQueries(dbContext));

    var response = await useCase.ExecuteAsync(tenantId, " 2026-ab-001 ", CancellationToken.None);

    Assert.NotNull(response);
    Assert.Equal(student.Id, response.StudentId);
    Assert.Equal(tenantId, response.TenantId);
    Assert.Equal("2026-AB-001", response.StudentNumber);
    Assert.Equal("Marco", response.FirstName);
    Assert.Equal("Morales", response.PaternalLastName);
    Assert.Equal("Castillo", response.MaternalLastName);
  }

  [Fact]
  public async Task ExecuteAsync_WhenStudentBelongsToAnotherTenant_ShouldReturnNull()
  {
    await using var dbContext = CreateDbContext();
    var student = new Student(Guid.NewGuid(), "2026-AB-001", "Marco", "Morales", "Castillo");

    await dbContext.Students.AddAsync(student);
    await dbContext.SaveChangesAsync();

    var useCase = new GetByNumberUseCase(new StudentQueries(dbContext));

    var response = await useCase.ExecuteAsync(Guid.NewGuid(), "2026-AB-001", CancellationToken.None);

    Assert.Null(response);
  }

  [Fact]
  public async Task ExecuteAsync_WhenStudentDoesNotExist_ShouldReturnNull()
  {
    await using var dbContext = CreateDbContext();
    var useCase = new GetByNumberUseCase(new StudentQueries(dbContext));

    var response = await useCase.ExecuteAsync(Guid.NewGuid(), "2026-AB-001", CancellationToken.None);

    Assert.Null(response);
  }

  private static SchoolControlDbContext CreateDbContext()
  {
    var options = new DbContextOptionsBuilder<SchoolControlDbContext>()
      .UseInMemoryDatabase(Guid.NewGuid().ToString())
      .Options;

    return new SchoolControlDbContext(options);
  }
}
