using Microsoft.EntityFrameworkCore;
using SIA.SchoolControlService.Application.Common.Exceptions.Students;
using SIA.SchoolControlService.Application.UseCases.Students;
using SIA.SchoolControlService.Contracts.Requests.Students;
using SIA.SchoolControlService.Infrastructure.Persistence.Contexts;
using SIA.SchoolControlService.Infrastructure.Persistence.DataStores;

namespace SIA.SchoolControlService.Tests.Application.Students;

public sealed class CreateUseCaseTests
{
  [Fact]
  public async Task ExecuteAsync_WithValidData_ShouldCreateStudent()
  {
    await using var dbContext = CreateDbContext();
    var useCase = new CreateUseCase(new StudentDataStore(dbContext));
    var tenantId = Guid.NewGuid();

    var response = await useCase.ExecuteAsync(tenantId, new CreateRequest
    {
      StudentNumber = " 2026-ab-001 ",
      FirstName = " Marco Antonio ",
      PaternalLastName = " Morales ",
      MaternalLastName = " Castillo "
    }, CancellationToken.None);

    Assert.NotEqual(Guid.Empty, response.StudentId);
    Assert.Equal(tenantId, response.TenantId);
    Assert.Equal("2026-AB-001", response.StudentNumber);
    Assert.Equal("Marco Antonio", response.FirstName);
    Assert.Equal("Morales", response.PaternalLastName);
    Assert.Equal("Castillo", response.MaternalLastName);
    Assert.True(response.Status);
    Assert.Single(dbContext.Students);
  }

  [Fact]
  public async Task ExecuteAsync_WhenNumberExistsInTenant_ShouldThrowConflict()
  {
    await using var dbContext = CreateDbContext();
    var useCase = new CreateUseCase(new StudentDataStore(dbContext));
    var tenantId = Guid.NewGuid();

    await useCase.ExecuteAsync(tenantId, new CreateRequest
    {
      StudentNumber = "2026-AB-001",
      FirstName = "Marco",
      PaternalLastName = "Morales",
      MaternalLastName = "Castillo"
    }, CancellationToken.None);

    var action = () => useCase.ExecuteAsync(tenantId, new CreateRequest
    {
      StudentNumber = " 2026-ab-001 ",
      FirstName = "María",
      PaternalLastName = "Hernández",
      MaternalLastName = "López"
    }, CancellationToken.None);

    await Assert.ThrowsAsync<DuplicateNumberException>(action);
    Assert.Single(dbContext.Students);
  }

  [Fact]
  public async Task ExecuteAsync_WhenNumberExistsInAnotherTenant_ShouldCreateStudent()
  {
    await using var dbContext = CreateDbContext();
    var useCase = new CreateUseCase(new StudentDataStore(dbContext));

    await useCase.ExecuteAsync(Guid.NewGuid(), new CreateRequest
    {
      StudentNumber = "2026-AB-001",
      FirstName = "Marco",
      PaternalLastName = "Morales",
      MaternalLastName = "Castillo"
    }, CancellationToken.None);

    var response = await useCase.ExecuteAsync(Guid.NewGuid(), new CreateRequest
    {
      StudentNumber = "2026-AB-001",
      FirstName = "María",
      PaternalLastName = "Hernández",
      MaternalLastName = "López"
    }, CancellationToken.None);

    Assert.Equal("2026-AB-001", response.StudentNumber);
    Assert.Equal(2, await dbContext.Students.CountAsync());
  }

  private static SchoolControlDbContext CreateDbContext()
  {
    var options = new DbContextOptionsBuilder<SchoolControlDbContext>()
      .UseInMemoryDatabase(Guid.NewGuid().ToString())
      .Options;

    return new SchoolControlDbContext(options);
  }
}
