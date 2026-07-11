using FluentValidation;

namespace CollegeControlSystem.Application.Departments.UpdateProgram
{
    internal sealed class UpdateProgramCommandValidator : AbstractValidator<UpdateProgramCommand>
    {
        public UpdateProgramCommandValidator()
        {
            //RuleFor(x => x.DepartmentId)
            //    .NotEmpty().WithMessage("DepartmentId cannot be empty.");

            RuleFor(x => x.ProgramId)
                .NotEmpty().WithMessage("ProgramId cannot be empty.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Program name is required.")
                .MaximumLength(150).WithMessage("Program name cannot exceed 150 characters.");
        }
    }
}
