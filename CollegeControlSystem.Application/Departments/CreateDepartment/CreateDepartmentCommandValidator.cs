using FluentValidation;
namespace CollegeControlSystem.Application.Departments.CreateDepartment
{
    public sealed class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
    {
        public CreateDepartmentCommandValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Department name is required.")
                .MaximumLength(100);

            RuleFor(c => c.Description)
                .MaximumLength(500);
        }
    }
}
