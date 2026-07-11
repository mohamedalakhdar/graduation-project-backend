using FluentValidation;
namespace CollegeControlSystem.Application.Departments.UpdateProgramCredits
{
    internal class UpdateProgramCreditsCommandValidator:AbstractValidator<UpdateProgramCreditsCommand>
    {
        public UpdateProgramCreditsCommandValidator()
        {
            //RuleFor(x => x.DepartmentId)
            //    .NotEmpty().WithMessage("DepartmentId cannot be empty.");
            RuleFor(x => x.ProgramId)
                .NotEmpty().WithMessage("ProgramId cannot be empty.");
            RuleFor(x => x.NewRequiredCredits)
                .GreaterThan(0).WithMessage("NewRequiredCredits must be greater than zero.");
        }
    }
}
