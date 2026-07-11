using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Application.Abstractions.Pagination;
using CollegeControlSystem.Domain.Abstractions;

namespace CollegeControlSystem.Application.Registrations.GetAllRegistrations;

internal sealed class GetAllRegistrationsQueryHandler : IQueryHandler<GetAllRegistrationsQuery, PagedResponse<RegistrationResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllRegistrationsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PagedResponse<RegistrationResponse>>> Handle(GetAllRegistrationsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _unitOfWork.RegistrationRepository.GetFilteredAsync(
            request.Status,
            request.StudentId,
            request.CourseOfferingId,
            request.Semester,
            request.Year,
            request.Page,
            request.PageSize,
            cancellationToken);

        var responseItems = items.Select(r => new RegistrationResponse(
            r.Id,
            r.StudentId,
            r.Student?.StudentName,
            r.CourseOfferingId,
            r.CourseOffering?.Course?.Code?.Value,
            r.CourseOffering?.Course?.Title,
            r.Status.ToString(),
            r.IsRetake,
            r.RegistrationDate,
            r.Grade?.LetterGrade
        )).ToList();

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return Result<PagedResponse<RegistrationResponse>>.Success(
            new PagedResponse<RegistrationResponse>(
                responseItems,
                request.Page,
                request.PageSize,
                totalCount,
                totalPages));
    }
}
