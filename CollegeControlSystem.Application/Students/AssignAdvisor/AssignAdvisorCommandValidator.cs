
using FluentValidation;

namespace CollegeControlSystem.Application.Students.AssignAdvisor
{
    internal class AssignAdvisorCommandValidator:AbstractValidator<AssignAdvisorCommand>
    {
        public AssignAdvisorCommandValidator()
        {
            RuleFor(x => x.StudentId).NotEmpty().WithMessage("StudentId cannot be empty.");
            RuleFor(x => x.AdvisorId).NotEmpty().WithMessage("AdvisorId cannot be empty.");
        }
    }
}
