using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Registrations;
using CollegeControlSystem.Domain.Students;

namespace CollegeControlSystem.Application.Registrations.ApproveRegistration
{
    internal sealed class ApproveRegistrationCommandHandler : ICommandHandler<ApproveRegistrationCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ApproveRegistrationCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ApproveRegistrationCommand request, CancellationToken cancellationToken)
        {
            // 1. Get Registration (Accessing Repo via UnitOfWork)
            var registration = await _unitOfWork.RegistrationRepository.GetByIdAsync(request.RegistrationId, cancellationToken);

            if (registration is null)
            {
                return Result.Failure(RegistrationErrors.NotFound);
            }

            // 2. Security Check: Is this Advisor allowed to approve this student?
            // We need the student to check who their advisor is.
            var student = await _unitOfWork.StudentRepository.GetByIdAsync(registration.StudentId, cancellationToken);

            if (student is null)
            {
                return Result.Failure(StudentErrors.StudentNotFound);
            }

            if (student.AdvisorId != request.AdvisorId)
            {
                return Result.Failure(RegistrationErrors.Unauthorized);
            }

            // 3. Domain Logic: Approve
            // This method (in Domain) checks if status is Pending
            var result = registration.Approve();

            if (result.IsFailure)
            {
                return result;
            }

            // 4. Commit Changes
            // Since we modified the 'registration' entity tracked by the context inside UoW
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
