using MassTransit;
using Microsoft.EntityFrameworkCore;
using SIA.AcademicService.Contracts.IntegrationEvents;
using SIA.SchoolControlService.Domain.Entities;
using SIA.SchoolControlService.Infrastructure.Persistence.Contexts;
using SIA.SchoolControlService.Infrastructure.Persistence.Entities;

namespace SIA.SchoolControlService.Infrastructure.MessageBus.Consumers;

public sealed class SubjectCreatedConsumer
    : IConsumer<SubjectCreatedIntegrationEvent>
{
  private readonly SchoolControlDbContext _dbContext;

  public SubjectCreatedConsumer(
      SchoolControlDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task Consume(
      ConsumeContext<SubjectCreatedIntegrationEvent> context)
  {
    var integrationEvent = context.Message;
    var cancellationToken = context.CancellationToken;

    var alreadyProcessed =
        await _dbContext.InboxMessages.AnyAsync(
            message => message.Id == integrationEvent.EventId,
            cancellationToken);

    if (alreadyProcessed)
    {
      return;
    }

    var inboxMessage = new InboxMessage(
        integrationEvent.EventId,
        $"{nameof(SubjectCreatedIntegrationEvent)}.v{integrationEvent.Version}",
        "SIA.AcademicService",
        integrationEvent.CorrelationId);

    var subjectReference = new SubjectReference(
        integrationEvent.SubjectId,
        integrationEvent.TenantId,
        integrationEvent.Code,
        integrationEvent.Name,
        integrationEvent.Credits,
        integrationEvent.Status ? "Active" : "Inactive");

    await using var transaction =
        await _dbContext.Database.BeginTransactionAsync(
            cancellationToken);

    try
    {
      await _dbContext.InboxMessages.AddAsync(
          inboxMessage,
          cancellationToken);

      await _dbContext.SubjectReferences.AddAsync(
          subjectReference,
          cancellationToken);

      inboxMessage.MarkAsProcessed();

      await _dbContext.SaveChangesAsync(
          cancellationToken);

      await transaction.CommitAsync(
          cancellationToken);
    }
    catch
    {
      await transaction.RollbackAsync(
          cancellationToken);

      throw;
    }
  }
}
