using FluentValidation;
namespace CollegeControlSystem.Application.Registrations.WithdrawCourse
{
    public sealed class WithdrawCourseCommandValidator : AbstractValidator<WithdrawCourseCommand>
    {
        public WithdrawCourseCommandValidator()
        {
            RuleFor(x => x.RegistrationId).NotEmpty();
            RuleFor(x => x.StudentId).NotEmpty();
        }
    }
}
