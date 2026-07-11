using FluentValidation;

namespace CollegeControlSystem.Application.Departments.AddProgram
{
    public sealed class AddProgramCommandValidator : AbstractValidator<AddProgramCommand>
    {
        public AddProgramCommandValidator()
        {
            RuleFor(c => c.DepartmentId).NotEmpty();
            RuleFor(c => c.Name).NotEmpty().MaximumLength(100);
            RuleFor(c => c.RequiredCredits).GreaterThan(0);
        }
    }
}
