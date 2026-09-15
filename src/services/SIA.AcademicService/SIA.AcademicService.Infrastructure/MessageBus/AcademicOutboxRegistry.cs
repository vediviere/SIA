using SIA.AcademicService.Contracts.IntegrationEvents;
using SIA.AcademicService.Contracts.IntegrationEvents.AcademicPeriods;
using SIA.AcademicService.Contracts.IntegrationEvents.EducationalPrograms;
using SIA.AcademicService.Contracts.IntegrationEvents.ServiceComplementaries;
using SIA.AcademicService.Contracts.IntegrationEvents.StudyPlans;
using SIA.AcademicService.Contracts.IntegrationEvents.StudyPlanSubjects;
using SIA.AcademicService.Contracts.IntegrationEvents.Subjects;
using SIA.BuildingBlocks.Messaging.Outbox;

namespace SIA.AcademicService.Infrastructure.MessageBus;

public static class AcademicOutboxRegistry
{
  public static OutboxEventRegistry Create()
  {
    return new OutboxEventRegistry()
      .Register<SubjectCreatedIntegrationEvent>(AcademicIntegrationEventTypes.SubjectCreatedV1)
      .Register<SubjectUpdatedIntegrationEvent>(AcademicIntegrationEventTypes.SubjectUpdatedV1)
      .Register<SubjectDeletedIntegrationEvent>(AcademicIntegrationEventTypes.SubjectDeletedV1)
      .Register<SubjectRestoredIntegrationEvent>(AcademicIntegrationEventTypes.SubjectRestoredV1)

      .Register<AcademicPeriodCreatedIntegrationEvent>(AcademicIntegrationEventTypes.AcademicPeriodCreatedV1)
      .Register<AcademicPeriodUpdatedIntegrationEvent>(AcademicIntegrationEventTypes.AcademicPeriodUpdatedV1)
      .Register<AcademicPeriodDeactivatedIntegrationEvent>(AcademicIntegrationEventTypes.AcademicPeriodDeactivatedV1)
      .Register<AcademicPeriodActivatedIntegrationEvent>(AcademicIntegrationEventTypes.AcademicPeriodActivatedV1)

      .Register<EducationalProgramCreatedIntegrationEvent>(AcademicIntegrationEventTypes.EducationalProgramCreatedV1)
      .Register<EducationalProgramUpdatedIntegrationEvent>(AcademicIntegrationEventTypes.EducationalProgramUpdatedV1)
      .Register<EducationalProgramDeactivatedIntegrationEvent>(AcademicIntegrationEventTypes.EducationalProgramDeactivatedV1)
      .Register<EducationalProgramRestoredIntegrationEvent>(AcademicIntegrationEventTypes.EducationalProgramRestoredV1)

      .Register<StudyPlanCreatedIntegrationEvent>(AcademicIntegrationEventTypes.StudyPlanCreatedV1)
      .Register<StudyPlanUpdatedIntegrationEvent>(AcademicIntegrationEventTypes.StudyPlanUpdatedV1)
      .Register<StudyPlanDeactivatedIntegrationEvent>(AcademicIntegrationEventTypes.StudyPlanDeactivatedV1)
      .Register<StudyPlanRestoredIntegrationEvent>(AcademicIntegrationEventTypes.StudyPlanRestoredV1)

      .Register<StudyPlanSubjectCreatedIntegrationEvent>(AcademicIntegrationEventTypes.StudyPlanSubjectCreatedV1)
      .Register<StudyPlanSubjectUpdatedIntegrationEvent>(AcademicIntegrationEventTypes.StudyPlanSubjectUpdatedV1)
      .Register<StudyPlanSubjectDeletedIntegrationEvent>(AcademicIntegrationEventTypes.StudyPlanSubjectDeletedV1)
      .Register<StudyPlanSubjectRestoredIntegrationEvent>(AcademicIntegrationEventTypes.StudyPlanSubjectRestoredV1)

      .Register<ServiceComplementaryCreatedIntegrationEvent>(AcademicIntegrationEventTypes.ServiceComplementaryCreatedV1)
      .Register<ServiceComplementaryUpdatedIntegrationEvent>(AcademicIntegrationEventTypes.ServiceComplementaryUpdatedV1)
      .Register<ServiceComplementaryDeletedIntegrationEvent>(AcademicIntegrationEventTypes.ServiceComplementaryDeletedV1)
      .Register<ServiceComplementaryRestoredIntegrationEvent>(AcademicIntegrationEventTypes.ServiceComplementaryRestoredV1);
  }
}
