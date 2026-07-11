using CollegeControlSystem.Application.Abstractions.Messaging;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Faculties;
using CollegeControlSystem.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CollegeControlSystem.Application.Faculties.CreateFaculty
{
    internal sealed class CreateFacultyCommandHandler : ICommandHandler<CreateFacultyCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> userManager;

        public CreateFacultyCommandHandler(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            this.userManager = userManager;
        }

        public async Task<Result<Guid>> Handle(CreateFacultyCommand request, CancellationToken cancellationToken)
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

            var role = request.IsAdvisor ? Roles.AdvisorRole : Roles.ProfessorRole;
            var res = await userManager.AddToRoleAsync(user, role);
            if (!res.Succeeded)
            {
                await userManager.DeleteAsync(user);
                return Result<Guid>.Failure(
                   IdentityErrors.RoleFailed
                );
            }
       

             var facultyResult = Faculty.Create(
                request.UserName,
                request.DepartmentId!,
                user.Id,
                request.Degree!
                );

            if (facultyResult.IsFailure) {
                await userManager.DeleteAsync(user); // 🔥 rollback
                return Result<Guid>.Failure(facultyResult.Error); 
            }
            await _unitOfWork.FacultyRepository.AddAsync(facultyResult?.Value);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(facultyResult.Value.Id);
        }
    }
}
