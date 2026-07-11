using FluentValidation;

namespace CollegeControlSystem.Application.Departments.DeleteDepartment
{
    internal sealed class DeleteDepartmentCommandValidator : AbstractValidator<DeleteDepartmentCommand>
    {
        public DeleteDepartmentCommandValidator()
        {
            RuleFor(x => x.DepartmentId)
                .NotEmpty()
                .WithMessage("DepartmentId is required.");
        }
    }
}
