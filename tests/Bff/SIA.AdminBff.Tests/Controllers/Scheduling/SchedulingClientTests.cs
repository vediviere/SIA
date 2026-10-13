
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using SIA.AdminBff.Clients.Scheduling.Clients;
using SIA.AdminBff.Clients.Scheduling.Dtos.Load;
using SIA.AdminBff.Clients.Scheduling.Dtos.Proposal;
using SIA.AdminBff.Clients.Scheduling.Dtos.SupportHour;
using SIA.AdminBff.Clients.Scheduling.Dtos.TeacherCandidate;
using SIA.AdminBff.Contracts.Scheduling.Enums;

namespace SIA.AdminBff.Tests.Clients.Scheduling;

public sealed class SchedulingClientTests
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task CreateProposalAsync_ShouldUseCorrectRouteAndPostMethod()
    {
        var response = CreateProposalDto();
        var handler = new RecordingHandler(JsonSerializer.Serialize(response, JsonOptions));
        using var httpClient = CreateHttpClient(handler);
        var client = new SchedulingClient(httpClient);

        await client.CreateProposalAsync(
            response.EducationalProgramId,
            response.AcademicPeriodId,
            response.DivisionHeadId,
            CancellationToken.None);

        AssertRequest(handler, HttpMethod.Post, "/api/academic-load-proposals");

        var body = await ReadRequestBodyAsync<ProposalCreateDto>(handler);
        Assert.Equal(response.EducationalProgramId, body.EducationalProgramId);
        Assert.Equal(response.AcademicPeriodId, body.AcademicPeriodId);
        Assert.Equal(response.DivisionHeadId, body.DivisionHeadId);
    }

    [Fact]
    public async Task SubmitForReviewAsync_ShouldUseCorrectRouteAndPostMethod()
    {
        var response = CreateProposalDto();
        var handler = new RecordingHandler(JsonSerializer.Serialize(response, JsonOptions));
        using var httpClient = CreateHttpClient(handler);
        var client = new SchedulingClient(httpClient);

        await client.SubmitForReviewAsync(response.Id, CancellationToken.None);

        AssertRequest(
            handler,
            HttpMethod.Post,
            $"/api/academic-load-proposals/{response.Id}/submit-for-review");
    }

    [Fact]
    public async Task GetTeacherCandidatesAsync_ShouldUseCorrectRouteAndGetMethod()
    {
        var programId = Guid.NewGuid();
        var handler = new RecordingHandler("[]");
        using var httpClient = CreateHttpClient(handler);
        var client = new SchedulingClient(httpClient);

        var result = await client.GetTeacherCandidatesAsync(
            programId,
            CancellationToken.None);

        AssertRequest(
            handler,
            HttpMethod.Get,
            $"/api/teacher-candidates?programId={programId}");

        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateLoadAsync_ShouldUseAcademicLoadRouteAndPostMethod()
    {
        var request = CreateLoadRequest();
        var response = CreateLoadDto(request);
        var handler = new RecordingHandler(JsonSerializer.Serialize(response, JsonOptions));
        using var httpClient = CreateHttpClient(handler);
        var client = new SchedulingClient(httpClient);

        var result = await client.CreateLoadAsync(request, CancellationToken.None);

        AssertRequest(handler, HttpMethod.Post, "/api/AcademicLoad");
        Assert.Equal(response.Id, result.Id);

        var body = await ReadRequestBodyAsync<LoadCreateDto>(handler);
        Assert.Equal(request.ProposalId, body.ProposalId);
        Assert.Equal(request.TeacherId, body.TeacherId);
        Assert.Equal(request.OfficialLetterNumber, body.OfficialLetterNumber);
    }

    [Fact]
    public async Task UpdateLoadAsync_ShouldUseAcademicLoadRouteAndPutMethod()
    {
        var loadId = Guid.NewGuid();
        var request = new LoadUpdateDto
        {
            OfficialLetterNumber = "OF-2026-002",
            ProposedDate = DateTime.UtcNow,
            AssignmentDate = DateTime.UtcNow.AddDays(1)
        };

        var response = CreateLoadDto(CreateLoadRequest()) with { Id = loadId };
        var handler = new RecordingHandler(JsonSerializer.Serialize(response, JsonOptions));
        using var httpClient = CreateHttpClient(handler);
        var client = new SchedulingClient(httpClient);

        var result = await client.UpdateLoadAsync(
            loadId,
            request,
            CancellationToken.None);

        AssertRequest(handler, HttpMethod.Put, $"/api/AcademicLoad/{loadId}");
        Assert.Equal(loadId, result.Id);

        var body = await ReadRequestBodyAsync<LoadUpdateDto>(handler);
        Assert.Equal(request.OfficialLetterNumber, body.OfficialLetterNumber);
        Assert.Equal(request.ProposedDate, body.ProposedDate);
        Assert.Equal(request.AssignmentDate, body.AssignmentDate);
    }

    [Fact]
    public async Task CreateSupportHourAsync_ShouldUseCorrectRouteAndPostMethod()
    {
        var request = new SupportHourCreateDto
        {
            ActivityId = Guid.NewGuid(),
            AcademicLoadId = Guid.NewGuid(),
            Hours = 4
        };

        var response = CreateSupportHourDto(request);
        var handler = new RecordingHandler(JsonSerializer.Serialize(response, JsonOptions));
        using var httpClient = CreateHttpClient(handler);
        var client = new SchedulingClient(httpClient);

        await client.CreateSupportHourAsync(request, CancellationToken.None);

        AssertRequest(handler, HttpMethod.Post, "/api/TeachingSupportHours");

        var body = await ReadRequestBodyAsync<SupportHourCreateDto>(handler);
        Assert.Equal(request.ActivityId, body.ActivityId);
        Assert.Equal(request.AcademicLoadId, body.AcademicLoadId);
        Assert.Equal(request.Hours, body.Hours);
    }

    [Fact]
    public async Task UpdateSupportHourAsync_ShouldUseCorrectRouteAndPutMethod()
    {
        var supportHourId = Guid.NewGuid();
        var request = new SupportHourUpdateDto { Hours = 6 };
        var response = CreateSupportHourDto(new SupportHourCreateDto
        {
            ActivityId = Guid.NewGuid(),
            AcademicLoadId = Guid.NewGuid(),
            Hours = request.Hours
        }) with
        { Id = supportHourId };

        var handler = new RecordingHandler(JsonSerializer.Serialize(response, JsonOptions));
        using var httpClient = CreateHttpClient(handler);
        var client = new SchedulingClient(httpClient);

        await client.UpdateSupportHourAsync(
            supportHourId,
            request,
            CancellationToken.None);

        AssertRequest(
            handler,
            HttpMethod.Put,
            $"/api/TeachingSupportHours/{supportHourId}");

        var body = await ReadRequestBodyAsync<SupportHourUpdateDto>(handler);
        Assert.Equal(request.Hours, body.Hours);
    }

    private static HttpClient CreateHttpClient(RecordingHandler handler)
    {
        return new HttpClient(handler)
        {
            BaseAddress = new Uri("https://scheduling.test/")
        };
    }

    private static void AssertRequest(
        RecordingHandler handler,
        HttpMethod expectedMethod,
        string expectedPath)
    {
        Assert.Equal(expectedMethod, handler.Method);
        Assert.Equal(expectedPath, handler.PathAndQuery);
    }

    private static async Task<T> ReadRequestBodyAsync<T>(RecordingHandler handler)
    {
        Assert.NotNull(handler.RequestBody);

        return JsonSerializer.Deserialize<T>(handler.RequestBody!, JsonOptions)
            ?? throw new InvalidOperationException("No se pudo deserializar el cuerpo HTTP.");
    }

    private static ProposalDto CreateProposalDto()
    {
        return new ProposalDto
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            EducationalProgramId = Guid.NewGuid(),
            AcademicPeriodId = Guid.NewGuid(),
            DivisionHeadId = Guid.NewGuid(),
            ProposalStatus = ProposalStatus.Draft,
            Status = true,
            CreatedAtUtc = DateTime.UtcNow,
            CorrelationId = Guid.NewGuid()
        };
    }

    private static LoadCreateDto CreateLoadRequest()
    {
        return new LoadCreateDto
        {
            ProposalId = Guid.NewGuid(),
            TeacherId = Guid.NewGuid(),
            DivisionHeadId = Guid.NewGuid(),
            AcademicPeriodId = Guid.NewGuid(),
            OfficialLetterNumber = "OF-2026-001",
            ProposedDate = DateTime.UtcNow,
            AssignmentDate = DateTime.UtcNow.AddDays(1)
        };
    }

    private static LoadDto CreateLoadDto(LoadCreateDto request)
    {
        return new LoadDto
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            ProposalId = request.ProposalId,
            TeacherId = request.TeacherId,
            DivisionHeadId = request.DivisionHeadId,
            AcademicPeriodId = request.AcademicPeriodId,
            OfficialLetterNumber = request.OfficialLetterNumber,
            ProposedDate = request.ProposedDate,
            ClassHours = 0,
            SupportHours = 0,
            AssignmentDate = request.AssignmentDate,
            Status = true,
            CreatedAtUtc = DateTime.UtcNow,
            CorrelationId = Guid.NewGuid()
        };
    }

    private static SupportHourDto CreateSupportHourDto(SupportHourCreateDto request)
    {
        return new SupportHourDto
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            ActivityId = request.ActivityId,
            AcademicLoadId = request.AcademicLoadId,
            Hours = request.Hours,
            Status = true,
            CreatedAtUtc = DateTime.UtcNow,
            CorrelationId = Guid.NewGuid()
        };
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        private readonly string _responseBody;

        public RecordingHandler(string responseBody)
        {
            _responseBody = responseBody;
        }

        public HttpMethod? Method { get; private set; }

        public string? PathAndQuery { get; private set; }

        public string? RequestBody { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Method = request.Method;
            PathAndQuery = request.RequestUri!.PathAndQuery;
            RequestBody = request.Content is null
                ? null
                : await request.Content.ReadAsStringAsync(cancellationToken);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    _responseBody,
                    System.Text.Encoding.UTF8,
                    "application/json")
            };
        }
    }
}
