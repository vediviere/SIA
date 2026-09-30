using System.Net;
using System.Net.Http.Json;
using SIA.AdminBff.Clients.Scheduling.Dtos.Load;
using SIA.AdminBff.Clients.Scheduling.Dtos.Proposal;
using SIA.AdminBff.Clients.Scheduling.Dtos.SupportHour;
using SIA.AdminBff.Clients.Scheduling.Dtos.TeacherCandidate;
using SIA.AdminBff.Configuration;
using SIA.AdminBff.Infrastructure.Errors;

namespace SIA.AdminBff.Clients.Scheduling.Clients;

public sealed class SchedulingClient : ISchedulingClient
{
    private readonly HttpClient _httpClient;

    public SchedulingClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyCollection<TeacherCandidateDto>> GetTeacherCandidatesAsync(Guid educationalProgramId, CancellationToken cancellationToken)
    {
        using var response = await _httpClient.GetAsync($"api/teacher-candidates?programId={educationalProgramId}", cancellationToken);

        response.EnsureInternalSuccess(InternalServiceConfiguration.SchedulingService);

        return await response.Content.ReadFromJsonAsync<IReadOnlyCollection<TeacherCandidateDto>>(cancellationToken: cancellationToken)
            ?? throw new InternalServiceException(InternalServiceConfiguration.SchedulingService, HttpStatusCode.BadGateway);
    }

    public async Task<ProposalDto> CreateProposalAsync(Guid educationalProgramId, Guid academicPeriodId, Guid divisionHeadId, CancellationToken cancellationToken)
    {
        var request = new ProposalCreateDto
        {
            EducationalProgramId = educationalProgramId,
            AcademicPeriodId = academicPeriodId,
            DivisionHeadId = divisionHeadId
        };

        using var response = await _httpClient.PostAsJsonAsync("api/academic-load-proposals", request, cancellationToken);

        response.EnsureInternalSuccess(InternalServiceConfiguration.SchedulingService);

        return await response.Content.ReadFromJsonAsync<ProposalDto>(cancellationToken: cancellationToken)
            ?? throw new InternalServiceException(InternalServiceConfiguration.SchedulingService, HttpStatusCode.BadGateway);
    }

    public async Task<LoadDto> CreateLoadAsync(LoadCreateDto request, CancellationToken cancellationToken)
    {
        using var response = await _httpClient.PostAsJsonAsync("api/academic-loads", request, cancellationToken);

        response.EnsureInternalSuccess(InternalServiceConfiguration.SchedulingService);

        return await response.Content.ReadFromJsonAsync<LoadDto>(cancellationToken: cancellationToken)
            ?? throw new InternalServiceException(InternalServiceConfiguration.SchedulingService, HttpStatusCode.BadGateway);
    }

    public async Task<LoadDto> UpdateLoadAsync(Guid loadId, LoadUpdateDto request, CancellationToken cancellationToken)
    {
        using var response = await _httpClient.PutAsJsonAsync($"api/academic-loads/{loadId}", request, cancellationToken);

        response.EnsureInternalSuccess(InternalServiceConfiguration.SchedulingService);

        return await response.Content.ReadFromJsonAsync<LoadDto>(cancellationToken: cancellationToken)
            ?? throw new InternalServiceException(InternalServiceConfiguration.SchedulingService, HttpStatusCode.BadGateway);
    }

    public async Task<ProposalDto> SubmitForReviewAsync(Guid proposalId, CancellationToken cancellationToken)
    {
        using var response = await _httpClient.PostAsync($"api/academic-load-proposals/{proposalId}/submit-for-review", null, cancellationToken);

        response.EnsureInternalSuccess(InternalServiceConfiguration.SchedulingService);

        return await response.Content.ReadFromJsonAsync<ProposalDto>(cancellationToken: cancellationToken)
            ?? throw new InternalServiceException(InternalServiceConfiguration.SchedulingService, HttpStatusCode.BadGateway);
    }

    public async Task<SupportHourDto> CreateSupportHourAsync(SupportHourCreateDto request, CancellationToken cancellationToken)
    {
        using var response = await _httpClient.PostAsJsonAsync("api/teaching-support-hours", request, cancellationToken);

        response.EnsureInternalSuccess(InternalServiceConfiguration.SchedulingService);

        return await response.Content.ReadFromJsonAsync<SupportHourDto>(cancellationToken: cancellationToken)
            ?? throw new InternalServiceException(InternalServiceConfiguration.SchedulingService, HttpStatusCode.BadGateway);
    }

    public async Task<SupportHourDto> UpdateSupportHourAsync(Guid supportHourId, SupportHourUpdateDto request, CancellationToken cancellationToken)
    {
        using var response = await _httpClient.PutAsJsonAsync($"api/teaching-support-hours/{supportHourId}", request, cancellationToken);

        response.EnsureInternalSuccess(InternalServiceConfiguration.SchedulingService);

        return await response.Content.ReadFromJsonAsync<SupportHourDto>(cancellationToken: cancellationToken)
            ?? throw new InternalServiceException(InternalServiceConfiguration.SchedulingService, HttpStatusCode.BadGateway);
    }
}