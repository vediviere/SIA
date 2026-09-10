using System.Text.Json.Serialization;

namespace SIA.SchedulingService.Application.Interfaces.ExternalServices;

public sealed record CandidateTeacherDto
{
    [JsonPropertyName("teacherId")]
    [JsonInclude]
    public Guid TeacherId { get; init; }

    [JsonPropertyName("id")]
    [JsonInclude]
    private Guid IdProp
    {
        init => TeacherId = TeacherId == default ? value : TeacherId;
    }

    [JsonPropertyName("professionalProfile")]
    public required string ProfessionalProfile { get; init; }

    [JsonPropertyName("programId")]
    public Guid? ProgramId { get; init; }

    [JsonPropertyName("contractHours")]
    public required int ContractHours { get; init; }

    [JsonPropertyName("status")]
    public required bool Status { get; init; }
}