using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Application.Abstractions.Pagination;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Courses;

namespace CollegeControlSystem.Application.Courses.GetCourseList
{
    internal sealed class GetCourseListQueryHandler : IQueryHandler<GetCourseListQuery, PagedResponse<GetCourseListQueryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCourseListQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagedResponse<GetCourseListQueryResponse>>> Handle(GetCourseListQuery request, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _unitOfWork.CourseRepository.GetFilteredAsync(
                request.Search,
                request.DepartmentId,
                request.MinCredits,
                request.MaxCredits,
                request.Page,
                request.PageSize,
                cancellationToken);

            var responseItems = items.Select(c => new GetCourseListQueryResponse(
                c.Id,
                c.Code.Value,
                c.Title,
                c.Credits
            )).ToList();

            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            return Result<PagedResponse<GetCourseListQueryResponse>>.Success(
                new PagedResponse<GetCourseListQueryResponse>(
                    responseItems,
                    request.Page,
                    request.PageSize,
                    totalCount,
                    totalPages));
        }
    }
}
