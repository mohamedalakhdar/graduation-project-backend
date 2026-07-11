using FluentValidation;

namespace CollegeControlSystem.Application.Identity.RevokeToken
{
    internal class RevokeTokenCommandValidator:AbstractValidator<RevokeTokenCommand>
    {
        public RevokeTokenCommandValidator()
        {
            RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("RefreshToken is required");
        }
    }
}
