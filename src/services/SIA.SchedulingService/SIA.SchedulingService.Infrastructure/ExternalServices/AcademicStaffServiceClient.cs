using System.Net.Http.Json;
using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.Interfaces.ExternalServices;

namespace SIA.SchedulingService.Infrastructure.ExternalServices;

public sealed class AcademicStaffServiceClient : IAcademicStaffServiceClient
{
    private readonly HttpClient _httpClient;

    public AcademicStaffServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<CandidateTeacherDto>> GetCandidateTeachersAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/teachers/candidates?tenantId={tenantId}", cancellationToken);

            response.EnsureSuccessStatusCode();

            var candidates = await response.Content.ReadFromJsonAsync<List<CandidateTeacherDto>>(cancellationToken: cancellationToken);

            return candidates ?? new List<CandidateTeacherDto>();
        }
        catch (HttpRequestException)
        {
            throw new AcademicStaffServiceUnavailableException();
        }
    }

    public async Task<CandidateTeacherDto?> GetTeacherAsync(Guid tenantId, Guid teacherId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/teachers/{tenantId}/{teacherId}", cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return await response.Content.ReadFromJsonAsync<CandidateTeacherDto>(options, cancellationToken);
        }
        catch (HttpRequestException)
        {
            throw new AcademicStaffServiceUnavailableException();
        }
    }
}