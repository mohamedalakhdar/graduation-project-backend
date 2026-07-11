namespace CollegeControlSystem.Application.Abstractions.Pagination;

public sealed record PagedResponse<T>(
    List<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages
);
