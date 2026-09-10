using System.Net;
using System.Net.Http.Json;
using SIA.AdminBff.Configuration;
using SIA.AdminBff.Infrastructure.Errors;

namespace SIA.AdminBff.Clients.Scheduling;

public sealed class SchedulingClient : ISchedulingClient
{
  private readonly HttpClient _httpClient;

  public SchedulingClient(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public async Task<IReadOnlyCollection<TeacherCandidateDto>> GetTeacherCandidatesAsync(Guid tenantId, Guid educationalProgramId, CancellationToken cancellationToken)
  {
    using var response = await _httpClient.GetAsync($"api/teacher-candidates?tenantId={tenantId}&programId={educationalProgramId}", cancellationToken);

    response.EnsureInternalSuccess(InternalServiceConfiguration.SchedulingService);

    return await response.Content.ReadFromJsonAsync<IReadOnlyCollection<TeacherCandidateDto>>(cancellationToken: cancellationToken)
        ?? throw new InternalServiceException(InternalServiceConfiguration.SchedulingService, HttpStatusCode.BadGateway);
  }

  public async Task<ProposalDto> CreateProposalAsync(Guid tenantId, Guid educationalProgramId, Guid academicPeriodId, Guid divisionHeadId, CancellationToken cancellationToken)
  {
    var request = new ProposalCreateDto
    {
      TenantId = tenantId,
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
    using var response = await _httpClient.PostAsJsonAsync("api/AcademicLoad", request, cancellationToken);

    response.EnsureInternalSuccess(InternalServiceConfiguration.SchedulingService);

    return await response.Content.ReadFromJsonAsync<LoadDto>(cancellationToken: cancellationToken)
        ?? throw new InternalServiceException(InternalServiceConfiguration.SchedulingService, HttpStatusCode.BadGateway);
  }

  public async Task<LoadDto> UpdateLoadAsync(Guid tenantId, Guid loadId, LoadUpdateDto request, CancellationToken cancellationToken)
  {
    using var response = await _httpClient.PutAsJsonAsync($"api/AcademicLoad/{tenantId}/{loadId}", request, cancellationToken);

    response.EnsureInternalSuccess(InternalServiceConfiguration.SchedulingService);

    return await response.Content.ReadFromJsonAsync<LoadDto>(cancellationToken: cancellationToken)
        ?? throw new InternalServiceException(InternalServiceConfiguration.SchedulingService, HttpStatusCode.BadGateway);
  }

  public async Task<ProposalDto> SubmitForReviewAsync(Guid tenantId, Guid proposalId, CancellationToken cancellationToken)
  {
    using var response = await _httpClient.PostAsync($"api/academic-load-proposals/{proposalId}/submit-for-review?tenantId={tenantId}", null, cancellationToken);

    response.EnsureInternalSuccess(InternalServiceConfiguration.SchedulingService);

    return await response.Content.ReadFromJsonAsync<ProposalDto>(cancellationToken: cancellationToken)
        ?? throw new InternalServiceException(InternalServiceConfiguration.SchedulingService, HttpStatusCode.BadGateway);
  }

  public async Task<SupportHourDto> CreateSupportHourAsync(SupportHourCreateDto request, CancellationToken cancellationToken)
  {
    using var response = await _httpClient.PostAsJsonAsync("api/TeachingSupportHoursController", request, cancellationToken);

    response.EnsureInternalSuccess(InternalServiceConfiguration.SchedulingService);

    return await response.Content.ReadFromJsonAsync<SupportHourDto>(cancellationToken: cancellationToken)
        ?? throw new InternalServiceException(InternalServiceConfiguration.SchedulingService, HttpStatusCode.BadGateway);
  }

  public async Task<SupportHourDto> UpdateSupportHourAsync(Guid tenantId, Guid supportHourId, SupportHourUpdateDto request, CancellationToken cancellationToken)
  {
    using var response = await _httpClient.PutAsJsonAsync($"api/TeachingSupportHoursController/{tenantId}/{supportHourId}", request, cancellationToken);

    response.EnsureInternalSuccess(InternalServiceConfiguration.SchedulingService);

    return await response.Content.ReadFromJsonAsync<SupportHourDto>(cancellationToken: cancellationToken)
        ?? throw new InternalServiceException(InternalServiceConfiguration.SchedulingService, HttpStatusCode.BadGateway);
  }
}
