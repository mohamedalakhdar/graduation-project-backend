using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;

namespace CollegeControlSystem.Application.Registrations.GetPendingRegistrations
{
    internal sealed class GetPendingRegistrationsQueryHandler : IQueryHandler<GetPendingRegistrationsQuery, List<PendingRegistrationResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPendingRegistrationsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<PendingRegistrationResponse>>> Handle(GetPendingRegistrationsQuery request, CancellationToken cancellationToken)
        {
            // 1. Fetch Pending Requests
            // This repository method must perform a join:
            // Registrations -> Students (Where Student.AdvisorId == request.AdvisorId)
            // AND Registration.Status == Pending

            var pendingRegistrations = await _unitOfWork.RegistrationRepository
                .GetPendingByAdvisorIdAsync(request.AdvisorId, cancellationToken);

            // 2. Map to Response
            // Note: We use null coalescing operators (??) just in case navigation properties are missing
            var response = pendingRegistrations.Select(r => new PendingRegistrationResponse(
                r.Id,
                r.StudentId,
                // Assuming your repo includes Student & CourseOffering.Course (via .Include)
                // If Student is null, we return "Unknown" to avoid crashing the UI
                r.Student?.StudentName ?? "Unknown Student",
                r.Student?.AcademicNumber ?? "N/A",
                r.CourseOffering?.Course?.Code?.Value ?? "N/A",
                r.CourseOffering?.Course?.Title ?? "Unknown Course",
                r.CourseOffering?.Semester?.Term.ToString() ?? "",
                r.CourseOffering?.Semester?.Year ?? 0,
                r.RegistrationDate
            )).ToList();

            return Result<List<PendingRegistrationResponse>>.Success(response);
        }
    }
}
