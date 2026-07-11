using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Identity;
using CollegeControlSystem.Domain.Students;
using Microsoft.AspNetCore.Identity;

namespace CollegeControlSystem.Application.Students.CreateStudent
{
    internal sealed class CreateStudentCommandHandler : ICommandHandler<CreateStudentCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> userManager;

        public CreateStudentCommandHandler(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            this.userManager = userManager;
        }

        public async Task<Result<Guid>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                return Result<Guid>.Failure(IdentityErrors.UserAlreadyExists);


            // Create a new user
            var user = new AppUser
            {
                UserName = request.Email,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
            };

            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var message = string.Join(
                    " | ",
                    result.Errors.Select(e => e.Description)
                );

                return Result<Guid>.Failure(
                    new Error("Identity.CreateFailed", message)
                );
            }

            var res = await userManager.AddToRoleAsync(user, Roles.StudentRole);
            if (!res.Succeeded)
            {
                await userManager.DeleteAsync(user);
                return Result<Guid>.Failure(
                   IdentityErrors.RoleFailed
                );
            }

            //Check for duplicate Academic Number
            var existingStudent = await _unitOfWork.StudentRepository.GetByAcademicNumberAsync(request.AcademicNumber, cancellationToken);
            if (existingStudent is not null)
            {
                await userManager.DeleteAsync(user);
                return Result<Guid>.Failure(StudentErrors.DuplicateAcademicNumber);
            }

            // check for duplicate National Num
            var isUnique = await _unitOfWork.StudentRepository.IsNationalIdUnique(request.NationalId, cancellationToken);
            if(!isUnique)
            {
                await userManager.DeleteAsync(user);
                return Result<Guid>.Failure(StudentErrors.DuplicateNationalId);
            }

            var studentResult = Student.Create(
                request.FullName,
                request.AcademicNumber,
                request.ProgramId,
                user.Id,
                request.NationalId);

            if (studentResult.IsFailure)
            {
                await userManager.DeleteAsync(user); // 🔥 rollback
                return Result<Guid>.Failure(studentResult.Error);
            }

            await _unitOfWork.StudentRepository.AddAsync(studentResult.Value);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(studentResult.Value.Id);
        }
    }
}