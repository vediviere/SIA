using SIA.SchedulingService.Application.DTOs.Classrooms;
using SIA.SchedulingService.Application.Interfaces.Queries;
using SIA.SchedulingService.Domain.Entities;


namespace SIA.SchedulingService.Application.UseCases.Classrooms;

public sealed class SearchClassroomsUseCase
{
    private readonly IClassroomQueries _queries;

    public SearchClassroomsUseCase(IClassroomQueries queries)
    {
        _queries = queries;
    }

    public async Task<IReadOnlyCollection<Classroom>> ExecuteAsync(
        ClassroomFilter filter,
        CancellationToken cancellationToken)
    {
        return await _queries.SearchAsync(filter, cancellationToken);
    }
}