using FluentValidation;

namespace CollegeControlSystem.Application.Departments.DeleteProgram
{
    internal sealed class DeleteProgramCommandValidator : AbstractValidator<DeleteProgramCommand>
    {
        public DeleteProgramCommandValidator()
        {
            //RuleFor(x => x.DepartmentId)
            //    .NotEmpty().WithMessage("DepartmentId cannot be empty.");

            RuleFor(x => x.ProgramId)
                .NotEmpty().WithMessage("ProgramId cannot be empty.");
        }
    }
}
