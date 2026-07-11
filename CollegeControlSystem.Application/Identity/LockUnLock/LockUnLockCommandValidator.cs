using FluentValidation;

namespace CollegeControlSystem.Application.Identity.LockUnLock
{
    internal class LockUnLockCommandValidator:AbstractValidator<LockUnLockCommand>
    {
        public LockUnLockCommandValidator()
        {
            RuleFor(x => x.userId)
                .NotEmpty().WithMessage("UserId is required.")
                .NotNull().WithMessage("UserId cannot be null.");
        }
    }
}
