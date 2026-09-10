using SIA.AcademicService.Application.Common.Exceptions;
using SIA.AcademicService.Application.DTOs.AcademicPeriod;
using SIA.AcademicService.Application.DTOs.EducationalProgram;
using SIA.AcademicService.Application.DTOs.StudyPlan;
using SIA.AcademicService.Application.Interfaces.Queries;
using SIA.AcademicService.Application.UseCases.AcademicContext;
using SIA.AcademicService.Contracts.Requests.AcademicContext;
using SIA.AcademicService.Domain.Entities;

namespace SIA.AcademicService.Tests.Application.UseCases.AcademicContext
{
    public sealed class GetAcademicContextUseCaseTests
    {
        [Fact]
        public async Task ExecuteAsync_WhenWithinPlanningWindow_ShouldReturnTrue()
        {
            var tenantId = Guid.NewGuid();
            var programId = Guid.NewGuid();
            var request = new GetAcademicContextRequest { TenantId = tenantId, EducationalProgramId = programId };

            var fakePeriodQueries = new FakeAcademicPeriodQueries(tenantId, new DateOnly(2026, 8, 10), new DateOnly(2026, 8, 20));
            var fakeProgramQueries = new FakeEducationalProgramQueries(tenantId, programId);
            var fakePlanQueries = new FakeStudyPlanQueries(tenantId, programId);

            var fakeTimeProvider = new FakeTimeProvider(new DateTime(2026, 8, 15, 12, 0, 0, DateTimeKind.Utc));

            var useCase = new GetAcademicContextUseCase(
                fakePeriodQueries,
                fakeProgramQueries,
                fakePlanQueries,
                fakeTimeProvider);

            var response = await useCase.ExecuteAsync(request, CancellationToken.None);

            Assert.NotNull(response);
            Assert.True(response.IsWithinPlanningWindow);
            Assert.Equal(tenantId, fakePeriodQueries.LastTenantIdRequested);
        }

        [Fact]
        public async Task ExecuteAsync_WhenOutsidePlanningWindow_ShouldReturnFalse()
        {
            var tenantId = Guid.NewGuid();
            var programId = Guid.NewGuid();
            var request = new GetAcademicContextRequest { TenantId = tenantId, EducationalProgramId = programId };

            var fakePeriodQueries = new FakeAcademicPeriodQueries(tenantId, new DateOnly(2026, 8, 10), new DateOnly(2026, 8, 20));
            var fakeProgramQueries = new FakeEducationalProgramQueries(tenantId, programId);
            var fakePlanQueries = new FakeStudyPlanQueries(tenantId, programId);

            var fakeTimeProvider = new FakeTimeProvider(new DateTime(2026, 8, 25, 12, 0, 0, DateTimeKind.Utc));

            var useCase = new GetAcademicContextUseCase(
                fakePeriodQueries,
                fakeProgramQueries,
                fakePlanQueries,
                fakeTimeProvider);

            var response = await useCase.ExecuteAsync(request, CancellationToken.None);

            Assert.NotNull(response);
            Assert.False(response.IsWithinPlanningWindow);
        }

        [Fact]
        public async Task ExecuteAsync_WhenNoActiveAcademicPeriod_ShouldThrowAcademicPeriodNotFoundException()
        {
            var request = new GetAcademicContextRequest { TenantId = Guid.NewGuid(), EducationalProgramId = Guid.NewGuid() };
            var fakePeriodQueries = new FakeAcademicPeriodQueries(Guid.NewGuid(), new DateOnly(), new DateOnly());

            var useCase = new GetAcademicContextUseCase(
                fakePeriodQueries,
                new FakeEducationalProgramQueries(Guid.NewGuid(), Guid.NewGuid()),
                new FakeStudyPlanQueries(Guid.NewGuid(), Guid.NewGuid()),
                new FakeTimeProvider(DateTime.UtcNow));

            await Assert.ThrowsAsync<AcademicPeriodNotFoundException>(() => useCase.ExecuteAsync(request, CancellationToken.None));
        }

        [Fact]
        public async Task ExecuteAsync_WhenTenantIdIsIncorrect_ShouldThrowAcademicPeriodNotFoundException()
        {
            var correctTenantId = Guid.NewGuid();
            var wrongTenantId = Guid.NewGuid();
            var request = new GetAcademicContextRequest { TenantId = wrongTenantId, EducationalProgramId = Guid.NewGuid() };

            var fakePeriodQueries = new FakeAcademicPeriodQueries(correctTenantId, new DateOnly(), new DateOnly());

            var useCase = new GetAcademicContextUseCase(
                fakePeriodQueries,
                new FakeEducationalProgramQueries(Guid.NewGuid(), Guid.NewGuid()),
                new FakeStudyPlanQueries(Guid.NewGuid(), Guid.NewGuid()),
                new FakeTimeProvider(DateTime.UtcNow));

            await Assert.ThrowsAsync<AcademicPeriodNotFoundException>(() => useCase.ExecuteAsync(request, CancellationToken.None));
        }

        [Fact]
        public async Task ExecuteAsync_WhenEducationalProgramNotFound_ShouldThrowEducationalProgramNotFoundException()
        {
            var tenantId = Guid.NewGuid();
            var request = new GetAcademicContextRequest { TenantId = tenantId, EducationalProgramId = Guid.NewGuid() };
            var fakePeriodQueries = new FakeAcademicPeriodQueries(tenantId, new DateOnly(), new DateOnly());
            var fakeProgramQueries = new FakeEducationalProgramQueries(tenantId, Guid.NewGuid());

            var useCase = new GetAcademicContextUseCase(
                fakePeriodQueries,
                fakeProgramQueries,
                new FakeStudyPlanQueries(tenantId, Guid.NewGuid()),
                new FakeTimeProvider(DateTime.UtcNow));

            await Assert.ThrowsAsync<EducationalProgramNotFoundException>(() => useCase.ExecuteAsync(request, CancellationToken.None));
        }

        [Fact]
        public async Task ExecuteAsync_WhenStudyPlanNotFound_ShouldThrowStudyPlanNotFoundException()
        {
            var tenantId = Guid.NewGuid();
            var programId = Guid.NewGuid();
            var request = new GetAcademicContextRequest { TenantId = tenantId, EducationalProgramId = programId };
            var fakePeriodQueries = new FakeAcademicPeriodQueries(tenantId, new DateOnly(), new DateOnly());
            var fakeProgramQueries = new FakeEducationalProgramQueries(tenantId, programId);
            var fakePlanQueries = new FakeStudyPlanQueries(tenantId, Guid.NewGuid());

            var useCase = new GetAcademicContextUseCase(
                fakePeriodQueries,
                fakeProgramQueries,
                fakePlanQueries,
                new FakeTimeProvider(DateTime.UtcNow));

            await Assert.ThrowsAsync<StudyPlanNotFoundException>(() => useCase.ExecuteAsync(request, CancellationToken.None));
        }

        private sealed class FakeAcademicPeriodQueries : IAcademicPeriodQueries
        {
            private readonly Guid _validTenantId;
            private readonly DateOnly _startDate;
            private readonly DateOnly _endDate;

            public Guid LastTenantIdRequested { get; private set; }

            public FakeAcademicPeriodQueries(Guid validTenantId, DateOnly startDate, DateOnly endDate)
            {
                _validTenantId = validTenantId;
                _startDate = startDate;
                _endDate = endDate;
            }

            public Task<AcademicPeriod?> GetActivePeriodAsync(Guid tenantId, CancellationToken cancellationToken)
            {
                LastTenantIdRequested = tenantId;
                if (tenantId != _validTenantId) return Task.FromResult<AcademicPeriod?>(null);

                var dummyDate = new DateOnly(2026, 1, 1);
                var period = new AcademicPeriod(tenantId, "CODE", "Name", dummyDate, dummyDate, _startDate, _endDate, dummyDate, dummyDate, dummyDate, dummyDate, dummyDate, dummyDate, dummyDate);
                return Task.FromResult<AcademicPeriod?>(period);
            }

            public Task<AcademicPeriod?> GetByIdAsync(Guid tenantId, Guid academicPeriodId, CancellationToken cancellationToken) => throw new NotImplementedException();
            public Task<IReadOnlyCollection<AcademicPeriod>> SearchAsync(AcademicPeriodFilter filter, CancellationToken cancellationToken) => throw new NotImplementedException();
        }

        private sealed class FakeEducationalProgramQueries : IEducationalProgramQueries
        {
            private readonly Guid _validTenantId;
            private readonly Guid _validProgramId;

            public FakeEducationalProgramQueries(Guid validTenantId, Guid validProgramId)
            {
                _validTenantId = validTenantId;
                _validProgramId = validProgramId;
            }

            public Task<EducationalProgram?> GetByIdAsync(Guid tenantId, Guid educationalProgramId, CancellationToken cancellationToken)
            {
                if (tenantId != _validTenantId || educationalProgramId != _validProgramId)
                    return Task.FromResult<EducationalProgram?>(null);

                return Task.FromResult<EducationalProgram?>(new EducationalProgram(tenantId, "CODE", "Name", "Level"));
            }

            public Task<IReadOnlyCollection<EducationalProgram>> SearchAsync(EducationalProgramFilter filter, CancellationToken cancellationToken) => throw new NotImplementedException();
        }

        private sealed class FakeStudyPlanQueries : IStudyPlanQueries
        {
            private readonly Guid _validTenantId;
            private readonly Guid _validProgramId;

            public FakeStudyPlanQueries(Guid validTenantId, Guid validProgramId)
            {
                _validTenantId = validTenantId;
                _validProgramId = validProgramId;
            }

            public Task<StudyPlan?> GetActiveByProgramIdAsync(Guid tenantId, Guid educationalProgramId, CancellationToken cancellationToken)
            {
                if (tenantId != _validTenantId || educationalProgramId != _validProgramId)
                    return Task.FromResult<StudyPlan?>(null);
                return Task.FromResult<StudyPlan?>(new StudyPlan(tenantId, educationalProgramId, "CODE", "Name", "V1", new DateOnly(2026, 1, 1)));
            }

            public Task<IReadOnlyCollection<StudyPlanSubjectDto>> GetSubjectsByStudyPlanAsync(Guid tenantId, Guid studyPlanId, CancellationToken cancellationToken)
            {
                return Task.FromResult<IReadOnlyCollection<StudyPlanSubjectDto>>(new List<StudyPlanSubjectDto>());
            }

            public Task<StudyPlan?> GetByIdAsync(Guid tenantId, Guid studyPlanId, CancellationToken cancellationToken) => throw new NotImplementedException();
            public Task<IReadOnlyCollection<StudyPlan>> SearchAsync(StudyPlanFilter filter, CancellationToken cancellationToken) => throw new NotImplementedException();
        }

        private sealed class FakeTimeProvider : TimeProvider
        {
            private readonly DateTime _utcNow;

            public FakeTimeProvider(DateTime utcNow)
            {
                _utcNow = utcNow;
            }

            public override DateTimeOffset GetUtcNow()
            {
                return new DateTimeOffset(_utcNow);
            }
        }
    }
}