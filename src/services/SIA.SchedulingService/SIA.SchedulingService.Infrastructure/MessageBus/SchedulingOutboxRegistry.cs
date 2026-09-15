using SIA.BuildingBlocks.Messaging.Outbox;
using SIA.SchedulingService.Contracts.IntegrationEvents;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoad;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoadProposal;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicOffering;
using SIA.SchedulingService.Contracts.IntegrationEvents.Building;
using SIA.SchedulingService.Contracts.IntegrationEvents.Classrooms;
using SIA.SchedulingService.Contracts.IntegrationEvents.ClassroomTypes;
using SIA.SchedulingService.Contracts.IntegrationEvents.ClassSchedule;
using SIA.SchedulingService.Contracts.IntegrationEvents.Group;
using SIA.SchedulingService.Contracts.IntegrationEvents.SupportActivity;
using SIA.SchedulingService.Contracts.IntegrationEvents.SupportSchedules;

namespace SIA.SchedulingService.Infrastructure.MessageBus;

public static class SchedulingOutboxRegistry
{
  public static OutboxEventRegistry Create()
  {
    return new OutboxEventRegistry()
      .Register<AcademicLoadCreatedIntegrationEvent>(SchedulingIntegrationEventTypes.AcademicLoadCreatedV1)
      .Register<AcademicLoadUpdatedIntegrationEvent>(SchedulingIntegrationEventTypes.AcademicLoadUpdatedV1)
      .Register<AcademicLoadDeactivatedIntegrationEvent>(SchedulingIntegrationEventTypes.AcademicLoadDeactivatedV1)
      .Register<AcademicLoadActivatedIntegrationEvent>(SchedulingIntegrationEventTypes.AcademicLoadActivatedV1)

      .Register<AcademicOfferingCreatedIntegrationEvent>(SchedulingIntegrationEventTypes.AcademicOfferingCreatedV1)
      .Register<AcademicOfferingUpdatedIntegrationEvent>(SchedulingIntegrationEventTypes.AcademicOfferingStatusUpdatedV1)
      .Register<AcademicOfferingDeactivatedIntegrationEvent>(SchedulingIntegrationEventTypes.AcademicOfferingDeactivatedV1)
      .Register<AcademicOfferingActivatedIntegrationEvent>(SchedulingIntegrationEventTypes.AcademicOfferingActivatedV1)

      .Register<BuildingCreatedIntegrationEvent>(SchedulingIntegrationEventTypes.BuildingCreatedV1)
      .Register<BuildingUpdatedIntegrationEvent>(SchedulingIntegrationEventTypes.BuildingUpdatedV1)
      .Register<BuildingDeactivatedIntegrationEvent>(SchedulingIntegrationEventTypes.BuildingDeactivatedV1)
      .Register<BuildingActivatedIntegrationEvent>(SchedulingIntegrationEventTypes.BuildingActivatedV1)

      .Register<GroupCreatedIntegrationEvent>(SchedulingIntegrationEventTypes.GroupCreatedV1)
      .Register<GroupUpdatedIntegrationEvent>(SchedulingIntegrationEventTypes.GroupUpdatedV1)
      .Register<GroupDeactivatedIntegrationEvent>(SchedulingIntegrationEventTypes.GroupDeactivatedV1)
      .Register<GroupActivateIntegrationEvent>(SchedulingIntegrationEventTypes.GroupActivatedV1)

      .Register<TeachingSupportHoursCreatedIntegrationEvent>(SchedulingIntegrationEventTypes.TeachingSupportHoursCreatedV1)
      .Register<TeachingSupportHoursUpdatedIntegrationEvent>(SchedulingIntegrationEventTypes.TeachingSupportHoursUpdatedV1)
      .Register<TeachingSupportHoursDeactivatedIntegrationEvent>(SchedulingIntegrationEventTypes.TeachingSupportHoursDeactivatedV1)
      .Register<TeachingSupportHoursActivatedIntegrationEvent>(SchedulingIntegrationEventTypes.TeachingSupportHoursActivatedV1)

      .Register<SupportActivityCreatedIntegrationEvent>(SchedulingIntegrationEventTypes.SupportActivityCreatedV1)
      .Register<SupportActivityUpdatedIntegrationEvent>(SchedulingIntegrationEventTypes.SupportActivityUpdatedV1)
      .Register<SupportActivityDeletedIntegrationEvent>(SchedulingIntegrationEventTypes.SupportActivityDeletedV1)
      .Register<SupportActivityRestoredIntegrationEvent>(SchedulingIntegrationEventTypes.SupportActivityRestoredV1)

      .Register<ClassScheduleCreatedIntegrationEvent>(SchedulingIntegrationEventTypes.ClassScheduleCreatedV1)
      .Register<ClassScheduleUpdatedIntegrationEvent>(SchedulingIntegrationEventTypes.ClassScheduleUpdatedV1)
      .Register<ClassScheduleDeletedIntegrationEvent>(SchedulingIntegrationEventTypes.ClassScheduleDeletedV1)
      .Register<ClassScheduleRestoredIntegrationEvent>(SchedulingIntegrationEventTypes.ClassScheduleRestoredV1)

      .Register<SupportScheduleCreatedIntegrationEvent>(SchedulingIntegrationEventTypes.SupportScheduleCreatedV1)
      .Register<SupportScheduleUpdatedIntegrationEvent>(SchedulingIntegrationEventTypes.SupportScheduleUpdatedV1)
      .Register<SupportScheduleDeletedIntegrationEvent>(SchedulingIntegrationEventTypes.SupportScheduleDeletedV1)
      .Register<SupportScheduleRestoredIntegrationEvent>(SchedulingIntegrationEventTypes.SupportScheduleRestoredV1)

      .Register<ClassroomLabCreatedIntegrationEvent>(SchedulingIntegrationEventTypes.ClassroomLabCreatedV1)
      .Register<ClassroomLabUpdatedIntegrationEvent>(SchedulingIntegrationEventTypes.ClassroomLabUpdatedV1)
      .Register<ClassroomLabDeletedIntegrationEvent>(SchedulingIntegrationEventTypes.ClassroomLabDeletedV1)
      .Register<ClassroomLabRestoredIntegrationEvent>(SchedulingIntegrationEventTypes.ClassroomLabRestoredV1)

      .Register<ClassroomTypeCreatedIntegrationEvent>(SchedulingIntegrationEventTypes.ClassroomTypeCreatedV1)
      .Register<ClassroomTypeUpdatedIntegrationEvent>(SchedulingIntegrationEventTypes.ClassroomTypeUpdatedV1)
      .Register<ClassroomTypeDeletedIntegrationEvent>(SchedulingIntegrationEventTypes.ClassroomTypeDeletedV1)
      .Register<ClassroomTypeRestoredIntegrationEvent>(SchedulingIntegrationEventTypes.ClassroomTypeRestoredV1)

      .Register<ProposalCreatedIntegrationEvent>(SchedulingIntegrationEventTypes.ProposalCreatedV1)
      .Register<ProposalSubmittedForReviewIntegrationEvent>(SchedulingIntegrationEventTypes.ProposalSubmittedForReviewV1);
  }
}
