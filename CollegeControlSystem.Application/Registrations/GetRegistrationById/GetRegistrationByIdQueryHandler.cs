using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Application.Registrations;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Registrations;

namespace CollegeControlSystem.Application.Registrations.GetRegistrationById
{
    internal sealed class GetRegistrationByIdQueryHandler : IQueryHandler<GetRegistrationByIdQuery, RegistrationResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRegistrationByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<RegistrationResponse>> Handle(GetRegistrationByIdQuery request, CancellationToken cancellationToken)
        {
            var registration = await _unitOfWork.RegistrationRepository.GetByIdAsync(request.RegistrationId, cancellationToken);

            if (registration is null)
            {
                return Result<RegistrationResponse>.Failure(RegistrationErrors.NotFound);
            }

            var response = new RegistrationResponse(
                registration.Id,
                registration.StudentId,
                registration.Student?.StudentName,
                registration.CourseOfferingId,
                registration.CourseOffering?.Course?.Code?.Value,
                registration.CourseOffering?.Course?.Title,
                registration.Status.ToString(),
                registration.IsRetake,
                registration.RegistrationDate,
                registration.Grade?.LetterGrade
            );

            return Result<RegistrationResponse>.Success(response);
        }
    }
}
