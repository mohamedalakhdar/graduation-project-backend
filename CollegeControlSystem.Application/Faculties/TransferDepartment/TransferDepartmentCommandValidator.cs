using FluentValidation;
namespace CollegeControlSystem.Application.Faculties.TransferDepartment
{
    internal class TransferDepartmentCommandValidator:AbstractValidator<TransferDepartmentCommand>
    { 
         public TransferDepartmentCommandValidator()
          {
                RuleFor(x => x.FacultyId).NotEmpty().WithMessage("FacultyId cannot be empty.");
                RuleFor(x => x.NewDepartmentId).NotEmpty().WithMessage("NewDepartmentId cannot be empty.");
        }
    }
}
