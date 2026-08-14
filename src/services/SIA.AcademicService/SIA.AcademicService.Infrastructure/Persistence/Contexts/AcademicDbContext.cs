using Microsoft.EntityFrameworkCore;
using SIA.AcademicService.Domain.Entities;
using SIA.BuildingBlocks.Messaging.Outbox;

namespace SIA.AcademicService.Infrastructure.Persistence.Contexts;

public sealed class AcademicDbContext : DbContext
{
    public AcademicDbContext(
        DbContextOptions<AcademicDbContext> options)
        : base(options)
    {
    }

    public DbSet<Subject> Subjects => Set<Subject>();

    public DbSet<StudyPlanSubject> StudyPlanSubjects => Set<StudyPlanSubject>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

  public DbSet<AcademicPeriod> AcademicPeriods => Set<AcademicPeriod>();

  public DbSet<EducationalProgram> EducationalPrograms => Set<EducationalProgram>();

  public DbSet<StudyPlan> StudyPlans => Set<StudyPlan>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AcademicDbContext).Assembly);
    }
}
