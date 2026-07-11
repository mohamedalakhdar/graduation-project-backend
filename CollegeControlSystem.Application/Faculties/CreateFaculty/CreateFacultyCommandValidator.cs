using FluentValidation;
namespace CollegeControlSystem.Application.Faculties.CreateFaculty
{
    internal class CreateFacultyCommandValidator : AbstractValidator<CreateFacultyCommand>
    {
        public CreateFacultyCommandValidator()
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

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full Name is required.")
                .MaximumLength(100).WithMessage("Full Name cannot exceed 100 characters.");
            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("Department ID is required.");

            RuleFor(x => x.Degree)
                .NotEmpty().WithMessage("Degree is required.")
                .IsInEnum().WithMessage("Degree must be a valid enum value.");
        }
    }
}
