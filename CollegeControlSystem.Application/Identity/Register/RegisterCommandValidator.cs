//using CollegeControlSystem.Domain.Identity;
//using FluentValidation;

//namespace CollegeControlSystem.Application.Identity.Register
//{
//    internal class RegisterCommandValidator:AbstractValidator<RegisterCommand>
//    {
//        public RegisterCommandValidator()
//        {
//            // 1. Common Validations
//            RuleFor(x => x.UserName)
//                .NotEmpty()
//                .MaximumLength(150)
//                .MinimumLength(3)
//                .WithMessage("Full name must be between 3 and 150 characters")
//                .Matches(@"^[a-zA-Z\s]+$")
//                .WithMessage("Full name can contain only letters and spaces"); ;
//            RuleFor(x => x.Email).NotEmpty().EmailAddress()
//                .NotEmpty().WithMessage("Email is required.")
//                .EmailAddress().WithMessage("Invalid email format.");
//            RuleFor(x => x.Password)
//                .NotEmpty().WithMessage("New password is required.")
//                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
//                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
//                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
//                .Matches("[0-9]").WithMessage("Password must contain at least one number.")
//                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

//            RuleFor(x =>x.PhoneNumber)
//                .Matches(@"^\+?[1-9]\d{1,14}$")
//                .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
//                .WithMessage("Invalid phone number format.");

//            RuleFor(x => x.Role).IsInEnum();

//            // 2. Student Validations
//            When(x => x.Role == Roles.StudentRole, () =>
//            {
//                RuleFor(x => x.AcademicNumber).NotEmpty()
//                    .WithMessage("Academic Number is required for students.");

//                RuleFor(x => x.NationalId).NotEmpty();

//                RuleFor(x => x.ProgramId).NotNull().NotEqual(Guid.Empty)
//                    .WithMessage("Program ID is required for students.");
//            });

//            // 3. Faculty (Instructor/Advisor) Validations
//            When(x => x.Role == Roles.ProfessorRole || x.Role == Roles.AdvisorRole, () =>
//            {
//                RuleFor(x => x.DepartmentId).NotNull().NotEqual(Guid.Empty)
//                    .WithMessage("Department ID is required for faculty.");

//                RuleFor(x => x.Degree).NotEmpty()
//                    .WithMessage("Degree is required for faculty.");
//            });
//        }
//    }
//}