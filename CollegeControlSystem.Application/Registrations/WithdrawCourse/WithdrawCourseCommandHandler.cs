using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Registrations;

namespace CollegeControlSystem.Application.Registrations.WithdrawCourse
{
    internal sealed class WithdrawCourseCommandHandler : ICommandHandler<WithdrawCourseCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public WithdrawCourseCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(WithdrawCourseCommand request, CancellationToken cancellationToken)
        {
            // 1. Get Registration
            var registration = await _unitOfWork.RegistrationRepository.GetByIdAsync(request.RegistrationId, cancellationToken);

            if (registration is null)
            {
                return Result.Failure(RegistrationErrors.NotFound);
            }

            // 2. Security Check: Ownership
            // Ensure the student withdrawing is the one who owns the registration
            if (registration.StudentId != request.StudentId)
            {
                return Result.Failure(RegistrationErrors.Unauthorized);
            }

            // 3. Domain Logic: Withdraw
            // This sets status to 'Withdrawn'. The Domain Entity handles validation (e.g. can't withdraw if already completed).
            var result = registration.Withdraw();

            if (result.IsFailure)
            {
                return result;
            }

            // 4. Commit
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
