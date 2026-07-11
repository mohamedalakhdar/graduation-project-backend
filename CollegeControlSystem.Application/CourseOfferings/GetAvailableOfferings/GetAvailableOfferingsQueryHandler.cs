using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Application.Abstractions.Pagination;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Shared;

namespace CollegeControlSystem.Application.CourseOfferings.GetAvailableOfferings
{
    internal sealed class GetAvailableOfferingsQueryHandler : IQueryHandler<GetAvailableOfferingsQuery, PagedResponse<OfferingQueryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAvailableOfferingsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagedResponse<OfferingQueryResponse>>> Handle(GetAvailableOfferingsQuery request, CancellationToken cancellationToken)
        {
            Semester? semester = null;

            if (!string.IsNullOrWhiteSpace(request.Term) && request.Year.HasValue)
            {
                var semesterResult = Semester.Create(request.Term, request.Year.Value);
                if (semesterResult.IsFailure)
                {
                    return Result<PagedResponse<OfferingQueryResponse>>.Failure(semesterResult.Error);
                }
                semester = semesterResult.Value;
            }

            var (items, totalCount) = await _unitOfWork.CourseOfferingRepository.GetFilteredAsync(
                semester,
                request.CourseId,
                request.InstructorId,
                request.Page,
                request.PageSize,
                cancellationToken);

            var responseItems = items.Select(o => new OfferingQueryResponse(
                o.Id,
                o.Course?.Code?.Value ?? "N/A",
                o.Course?.Title ?? "Unknown Title",
                o.Instructor?.FacultyName ?? "Unknown Instructor",
                o.Capacity,
                o.CurrentEnrolled,
                o.IsFull
            )).ToList();

            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            return Result<PagedResponse<OfferingQueryResponse>>.Success(
                new PagedResponse<OfferingQueryResponse>(
                    responseItems,
                    request.Page,
                    request.PageSize,
                    totalCount,
                    totalPages));
        }
    }
}
