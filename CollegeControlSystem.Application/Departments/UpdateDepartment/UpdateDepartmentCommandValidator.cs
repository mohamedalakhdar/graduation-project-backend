using FluentValidation;
namespace CollegeControlSystem.Application.Departments.UpdateDepartment
{
    internal class UpdateDepartmentCommandValidator:AbstractValidator<UpdateDepartmentCommand>
    {
        public UpdateDepartmentCommandValidator()
        {
            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("Department ID cannot be empty.");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Department name cannot be empty.")
                .MaximumLength(100).WithMessage("Department name cannot exceed 100 characters.");
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        }
    }
}
