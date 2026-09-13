
namespace SIA.AcademicService.Application.DTOs.AcademicPeriod
{
    public sealed record AcademicPeriodFilter
    {

        public string? Code { get; init; }

        public string? Name { get; init; }

        public bool? Status { get; init; }

        public int Page { get; init; } = 1;

        public int PageSize { get; init; } = 10;
    }
}
