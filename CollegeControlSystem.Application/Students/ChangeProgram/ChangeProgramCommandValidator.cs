using FluentValidation;

namespace CollegeControlSystem.Application.Students.ChangeProgram
{
    internal class ChangeProgramCommandValidator : AbstractValidator<ChangeProgramCommand>
    {
        public ChangeProgramCommandValidator()
        {
            RuleFor(x => x.StudentId).NotEmpty().WithMessage("StudentId cannot be empty.");
            RuleFor(x => x.NewProgramId).NotEmpty().WithMessage("NewProgramId cannot be empty.");
        }
    }
}
