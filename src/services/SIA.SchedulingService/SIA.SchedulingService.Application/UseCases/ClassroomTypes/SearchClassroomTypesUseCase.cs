using SIA.SchedulingService.Application.DTOs.ClassroomTypes;
using SIA.SchedulingService.Application.Interfaces.Queries;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Application.UseCases.ClassroomTypes;

public sealed class SearchClassroomTypesUseCase
{
    private readonly IClassroomTypeQueries _queries;

    public SearchClassroomTypesUseCase(IClassroomTypeQueries queries)
    {
        _queries = queries;
    }

    public async Task<IReadOnlyCollection<ClassroomType>> ExecuteAsync(
        ClassroomTypeFilter filter,
        CancellationToken cancellationToken)
    {
        return await _queries.SearchAsync(filter, cancellationToken);
    }
}
