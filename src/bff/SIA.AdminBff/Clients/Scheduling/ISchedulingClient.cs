namespace SIA.AdminBff.Clients.Scheduling;

public interface ISchedulingClient
{
  Task<IReadOnlyCollection<TeacherCandidateDto>> GetTeacherCandidatesAsync(Guid tenantId, Guid educationalProgramId, CancellationToken cancellationToken);

  Task<ProposalDto> CreateProposalAsync(Guid tenantId, Guid educationalProgramId, Guid academicPeriodId, Guid divisionHeadId, CancellationToken cancellationToken);

  Task<LoadDto> CreateLoadAsync(LoadCreateDto request, CancellationToken cancellationToken);

  Task<LoadDto> UpdateLoadAsync(Guid tenantId, Guid loadId, LoadUpdateDto request, CancellationToken cancellationToken);

  Task<ProposalDto> SubmitForReviewAsync(Guid tenantId, Guid proposalId, CancellationToken cancellationToken);

  Task<SupportHourDto> CreateSupportHourAsync(SupportHourCreateDto request, CancellationToken cancellationToken);

  Task<SupportHourDto> UpdateSupportHourAsync(Guid tenantId, Guid supportHourId, SupportHourUpdateDto request, CancellationToken cancellationToken);
}
