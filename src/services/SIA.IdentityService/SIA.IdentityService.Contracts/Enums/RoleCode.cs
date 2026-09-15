using System.Text.Json.Serialization;

namespace SIA.IdentityService.Contracts.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<RoleCode>))]
public enum RoleCode
{
  Administrator = 1,
  Teacher = 2,
  DivisionHead = 3,
  Coordinator = 4,
  Student = 5
}
