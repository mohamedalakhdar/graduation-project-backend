using FluentValidation;

namespace CollegeControlSystem.Application.Identity.RefreshToken
{
    public class RefreshTokenCommandValidator:AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("RefreshToken is required");
        }
    }
}
