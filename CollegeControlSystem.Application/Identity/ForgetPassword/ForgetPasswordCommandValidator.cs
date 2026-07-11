using FluentValidation;

namespace CollegeControlSystem.Application.Identity.ForgetPassword
{
    internal class ForgetPasswordValidator : AbstractValidator<ForgetPasswordCommand>
    {
        public ForgetPasswordValidator()
        {
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.BaseUrl)
                .NotEmpty().WithMessage("Base URL is required");
        }
    }
}
