using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Application.CourseOfferings.GetAvailableOfferings;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.CourseOfferings;

namespace CollegeControlSystem.Application.CourseOfferings.GetOfferingById;

internal sealed class GetOfferingByIdQueryHandler : IQueryHandler<GetOfferingByIdQuery, OfferingQueryResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetOfferingByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OfferingQueryResponse>> Handle(GetOfferingByIdQuery request, CancellationToken cancellationToken)
    {
        var offering = await _unitOfWork.CourseOfferingRepository.GetByIdAsync(request.OfferingId, cancellationToken);

        if (offering is null)
            return Result<OfferingQueryResponse>.Failure(CourseOfferingErrors.OfferingNotFound);

        var response = new OfferingQueryResponse(
            offering.Id,
            offering.Course.Code.Value,
            offering.Course.Title,
            offering.Instructor.FacultyName,
            offering.Capacity,
            offering.CurrentEnrolled,
            offering.IsFull);

        return Result<OfferingQueryResponse>.Success(response);
    }
}
