//using CollegeControlSystem.Application.Abstractions.Messaging;
//using CollegeControlSystem.Domain.Abstractions;
//using CollegeControlSystem.Domain.Faculties;
//using CollegeControlSystem.Domain.Identity;
//using CollegeControlSystem.Domain.Students;
//using Microsoft.AspNetCore.Identity;

//namespace CollegeControlSystem.Application.Identity.Register
//{
//    internal class RegisterCommandHandler : ICommandHandler<RegisterCommand, AuthResponse>
//    {
//        private readonly UserManager<AppUser> userManager;
//        private readonly ITokenGenerator tokenGenerator;
//        private readonly IUnitOfWork uow;
//        public RegisterCommandHandler(IUnitOfWork unitOfWork,
//            UserManager<AppUser> userManager,
//            ITokenGenerator tokenGenerator
//            )
//        {
//            this.uow = unitOfWork;
//            this.userManager = userManager;
//            this.tokenGenerator = tokenGenerator;
//        }
//        public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
//        {
//            var existingUser = await userManager.FindByEmailAsync(request.Email);
//            if (existingUser != null)
//                return Result<AuthResponse>.Failure(IdentityErrors.UserAlreadyExists);


//            // Create a new user
//            var user = new AppUser
//            {
//                UserName = request.Email,
//                Email = request.Email,
//                PhoneNumber = request.PhoneNumber,
//            };

//            var result = await userManager.CreateAsync(user, request.Password);
//            if (!result.Succeeded)
//            {
//                var message = string.Join(
//                    " | ",
//                    result.Errors.Select(e => e.Description)
//                );

//                return Result<AuthResponse>.Failure(
//                    new Error("Identity.CreateFailed", message)
//                );
//            }

//            switch (request.Role)
//            {

//                case Roles.StudentRole:
//                    // add role to user
//                    //await userManager.AddToRoleAsync(user, Roles.StudentRole);
//                    var roleResult = await userManager.AddToRoleAsync(user, Roles.StudentRole);
//                    if (!roleResult.Succeeded)
//                    {
//                        await userManager.DeleteAsync(user);
//                        return Result<AuthResponse>.Failure(
//                           IdentityErrors.RoleFailed
//                        );
//                    }
//                    var studentResult = Student.Create(
//                        request.UserName,
//                        request.AcademicNumber!,
//                        request.ProgramId!.Value,
//                        user.Id,
//                        request.NationalId!
//                    );

//                    if (studentResult.IsFailure)
//                    {
//                        await userManager.DeleteAsync(user); // 🔥 rollback
//                        return Result<AuthResponse>.Failure(studentResult.Error);
//                    }
//                    await uow.StudentRepository.AddAsync(studentResult?.Value);
//                    break;

//                case Roles.InstructorRole:
//                case Roles.AdvisorRole:
//                    // add role to user
//                    if (request.Role == Roles.AdvisorRole)
//                    //await userManager.AddToRoleAsync(user, Roles.AdvisorRole);
//                    {
//                        var res = await userManager.AddToRoleAsync(user, Roles.AdvisorRole);
//                        if (!res.Succeeded)
//                        {
//                            await userManager.DeleteAsync(user);
//                            return Result<AuthResponse>.Failure(
//                               IdentityErrors.RoleFailed
//                            );
//                        }
//                    }
//                    else if (request.Role == Roles.InstructorRole)
//                    //await userManager.AddToRoleAsync(user, Roles.InstructorRole);
//                    {
//                        var res = await userManager.AddToRoleAsync(user, Roles.InstructorRole);
//                        if (!res.Succeeded)
//                        {
//                            await userManager.DeleteAsync(user);
//                            return Result<AuthResponse>.Failure(
//                               IdentityErrors.RoleFailed
//                            );
//                        }
//                    }


//                    var facultyResult = Faculty.Create(
//                            request.UserName,
//                            request.DepartmentId!.Value,
//                            user.Id,
//                            request.Degree!
//                        );

//                    if (facultyResult.IsFailure) {
//                        await userManager.DeleteAsync(user); // 🔥 rollback
//                        return Result<AuthResponse>.Failure(facultyResult.Error); 
//                    }
//                    await uow.FacultieRepository.AddAsync(facultyResult?.Value);

//                    break;

//                case Roles.AdminRole:
//                    // Admins might be basic Users or have their own Entity. 
//                    // Assuming basic User for now, or you'd have an Admin entity.
//                    // For this example, we return error as Admin creation usually has a separate flow or seeding.
//                    return Result<AuthResponse>.Failure(IdentityErrors.AdminNotAllowed);

//                default:
//                    return Result<AuthResponse>.Failure(IdentityErrors.InvalidRole);
//            }


//            await uow.SaveChangesAsync();




//            // generate token
//            var token = await tokenGenerator.GenerateJwtTokenAsync(user);
//            // generate refreshToken
//            var refreshToken = tokenGenerator.GenereteRefreshToken();


//            // then save it in db
//            user.RefreshTokens.Add(refreshToken);
//            await userManager.UpdateAsync(user);

//            var authResponse = new AuthResponse
//            {
//                RefreshToken = refreshToken.Token,
//                RefreshTokenExpiresOn = refreshToken.ExpiresOn,
//                Token = token
//            };
//            return Result<AuthResponse>.Success(authResponse);
//        }
    
//    } 
//}
