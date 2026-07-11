using FluentValidation;
namespace CollegeControlSystem.Application.Students.CreateStudent
{
    public sealed class CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>
    {
        public CreateStudentCommandValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .MaximumLength(150)
                .MinimumLength(3)
                .WithMessage("Full name must be between 3 and 150 characters")
                .Matches(@"^[a-zA-Z\s]+$")
                .WithMessage("Full name can contain only letters and spaces"); ;
            RuleFor(x => x.Email).NotEmpty().EmailAddress()
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one number.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
                .WithMessage("Invalid phone number format.");

            RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.AcademicNumber).NotEmpty().MaximumLength(20);
            RuleFor(x => x.NationalId).NotEmpty().Length(14).Matches(@"^\d+$").WithMessage("National ID must be 14 digits.");
            RuleFor(x => x.ProgramId).NotEmpty();
        }
    }
}
