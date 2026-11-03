using System.Text.Json.Serialization;

namespace SIA.AdminBff.Clients.Workflow;

[JsonConverter(typeof(JsonStringEnumConverter<ObservationTarget>))]
public enum ObservationTarget
{
  Proposal = 1,
  Offering = 2,
  TeacherAssignment = 3,
  TeachingHours = 4,
  SupportActivity = 5
}
