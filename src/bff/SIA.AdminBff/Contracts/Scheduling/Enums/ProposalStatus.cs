using System.Text.Json.Serialization;

namespace SIA.AdminBff.Contracts.Scheduling.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProposalStatus
{
  Draft = 1,
  SubmittedForReview = 2,
  Approved = 3,
  Rejected = 4
}
