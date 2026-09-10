using SIA.SchedulingService.Application.Interfaces.ExternalServices;
using SIA.SchedulingService.Application.UseCases.Teachers;

namespace SIA.SchedulingService.Tests.Application.UseCases.Teachers;

public sealed class GetCandidateTeachersUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithProgramId_ShouldPrioritizeMatchingProgram()
    {
        var tenantId = Guid.NewGuid();
        var programId = Guid.NewGuid();

        var candidates = new List<CandidateTeacherDto>
        {
            new() { TeacherId = Guid.NewGuid(), ProfessionalProfile = "Otro perfil", ProgramId = null, ContractHours = 40, Status = true },
            new() { TeacherId = Guid.NewGuid(), ProfessionalProfile = "Perfil del programa", ProgramId = programId, ContractHours = 40, Status = true }
        };

        var client = new FakeAcademicStaffServiceClient(candidates);
        var useCase = new GetCandidateTeachersUseCase(client);

        var result = await useCase.ExecuteAsync(tenantId, programId, CancellationToken.None);

        Assert.Equal(programId, result.First().ProgramId);
    }

    [Fact]
    public async Task ExecuteAsync_WithoutProgramId_ShouldReturnAllCandidates()
    {
        var candidates = new List<CandidateTeacherDto>
        {
            new() { TeacherId = Guid.NewGuid(), ProfessionalProfile = "Perfil A", ProgramId = null, ContractHours = 40, Status = true }
        };

        var client = new FakeAcademicStaffServiceClient(candidates);
        var useCase = new GetCandidateTeachersUseCase(client);

        var result = await useCase.ExecuteAsync(Guid.NewGuid(), null, CancellationToken.None);

        Assert.Single(result);
    }

    private sealed class FakeAcademicStaffServiceClient : IAcademicStaffServiceClient
    {
        private readonly IReadOnlyList<CandidateTeacherDto> _candidates;

        public FakeAcademicStaffServiceClient(IReadOnlyList<CandidateTeacherDto> candidates)
        {
            _candidates = candidates;
        }

        public Task<IReadOnlyList<CandidateTeacherDto>> GetCandidateTeachersAsync(Guid tenantId, CancellationToken cancellationToken)
            => Task.FromResult(_candidates);

        public Task<CandidateTeacherDto?> GetTeacherAsync(Guid tenantId, Guid teacherId, CancellationToken cancellationToken)
            => Task.FromResult(_candidates.FirstOrDefault(c => c.TeacherId == teacherId));
    }
}