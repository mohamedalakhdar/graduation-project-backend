using FluentValidation;
namespace CollegeControlSystem.Application.Registrations.ApproveRegistration
{
    public sealed class ApproveRegistrationCommandValidator : AbstractValidator<ApproveRegistrationCommand>
    {
        public ApproveRegistrationCommandValidator()
        {
            RuleFor(x => x.RegistrationId).NotEmpty();
            RuleFor(x => x.AdvisorId).NotEmpty();
        }
    }
}
