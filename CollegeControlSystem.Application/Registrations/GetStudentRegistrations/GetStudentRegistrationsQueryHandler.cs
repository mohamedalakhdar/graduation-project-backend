using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Application.Registrations;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Registrations;
using CollegeControlSystem.Domain.Students;

namespace CollegeControlSystem.Application.Registrations.GetStudentRegistrations
{
    internal sealed class GetStudentRegistrationsQueryHandler : IQueryHandler<GetStudentRegistrationsQuery, List<RegistrationResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetStudentRegistrationsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<RegistrationResponse>>> Handle(GetStudentRegistrationsQuery request, CancellationToken cancellationToken)
        {
            var student = await _unitOfWork.StudentRepository.GetByIdAsync(request.StudentId, cancellationToken);
            if (student is null)
            {
                return Result<List<RegistrationResponse>>.Failure(StudentErrors.StudentNotFound);
            }

            var registrations = await _unitOfWork.RegistrationRepository.GetAllByStudentIdAsync(request.StudentId, cancellationToken);

            var response = registrations.Select(r => new RegistrationResponse(
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

            return Result<List<RegistrationResponse>>.Success(response);
        }
    }
}
