using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Application.Abstractions.Pagination;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Faculties;

namespace CollegeControlSystem.Application.Faculties.GetAdvisorsList
{
    internal sealed class GetAdvisorsListQueryHandler : IQueryHandler<GetAdvisorsListQuery, PagedResponse<GetAdvisorsListQueryResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetAdvisorsListQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Result<PagedResponse<GetAdvisorsListQueryResponse>>> Handle(GetAdvisorsListQuery request, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _uow.FacultyRepository.GetAdvisorsFilteredAsync(
                request.Search,
                request.DepartmentId,
                request.Page,
                request.PageSize,
                cancellationToken);

            var responseItems = items.Select(f => new GetAdvisorsListQueryResponse(
                f.Id,
                f.FacultyName,
                f.Degree.ToString(),
                f.Department?.DepartmentName ?? "Unknown",
                f.AppUser?.Email ?? "Unknown"
            )).ToList();

            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            return Result<PagedResponse<GetAdvisorsListQueryResponse>>.Success(
                new PagedResponse<GetAdvisorsListQueryResponse>(
                    responseItems,
                    request.Page,
                    request.PageSize,
                    totalCount,
                    totalPages));
        }
    }
}
