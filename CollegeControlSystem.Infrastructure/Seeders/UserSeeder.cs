using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Faculties;
using CollegeControlSystem.Domain.Identity;
using CollegeControlSystem.Domain.Students;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CollegeControlSystem.Infrastructure.Seeders;

public static class UserSeeder
{
    public const string DefaultPassword = "Test@123";

    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

        await SeedStudentAsync(userManager, unitOfWork);
        await SeedProfessorAsync(userManager, unitOfWork);
        await SeedAdvisorAsync(userManager, unitOfWork);
    }

    private static async Task SeedStudentAsync(UserManager<AppUser> userManager, IUnitOfWork unitOfWork)
    {
        const string email = "student@test.com";
        if (await userManager.FindByEmailAsync(email) != null)
            return;

        var programs = await unitOfWork.DepartmentRepository.GetProgramsWithDepartmentAsync();
        var program = programs.FirstOrDefault(p => p.Name == "Computers and Automatic Control Engineering");

        if (program == null)
            return;

        var user = new AppUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            PhoneNumber = "01000000000"
        };

        var result = await userManager.CreateAsync(user, DefaultPassword);
        if (!result.Succeeded)
            return;

        await userManager.AddToRoleAsync(user, Roles.StudentRole);

        var studentResult = Student.Create(
            "Ahmed Hassan",
            "CS2024001",
            program.Id,
            user.Id,
            "12345678901234"
        );

        if (studentResult.IsSuccess)
        {
            await unitOfWork.StudentRepository.AddAsync(studentResult.Value);
            await unitOfWork.SaveChangesAsync();
        }
    }

    private static async Task SeedProfessorAsync(UserManager<AppUser> userManager, IUnitOfWork unitOfWork)
    {
        const string email = "professor@test.com";
        if (await userManager.FindByEmailAsync(email) != null)
            return;

        var department = await unitOfWork.DepartmentRepository.GetByNameAsync("Electrical Engineering");
        if (department == null)
            return;

        var user = new AppUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            PhoneNumber = "01000000001"
        };

        var result = await userManager.CreateAsync(user, DefaultPassword);
        if (!result.Succeeded)
            return;

        await userManager.AddToRoleAsync(user, Roles.ProfessorRole);

        var facultyResult = Faculty.Create(
            "Dr. Sarah Mohamed",
            department.Id,
            user.Id,
            FacultyDegree.Professor
        );

        if (facultyResult.IsSuccess)
        {
            await unitOfWork.FacultyRepository.AddAsync(facultyResult.Value);
            await unitOfWork.SaveChangesAsync();
        }
    }

    private static async Task SeedAdvisorAsync(UserManager<AppUser> userManager, IUnitOfWork unitOfWork)
    {
        const string email = "advisor@test.com";
        if (await userManager.FindByEmailAsync(email) != null)
            return;

        var department = await unitOfWork.DepartmentRepository.GetByNameAsync("Electrical Engineering");
        if (department == null)
            return;

        var user = new AppUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            PhoneNumber = "01000000002"
        };

        var result = await userManager.CreateAsync(user, DefaultPassword);
        if (!result.Succeeded)
            return;

        await userManager.AddToRoleAsync(user, Roles.AdvisorRole);

        var facultyResult = Faculty.Create(
            "Dr. Omar Ibrahim",
            department.Id,
            user.Id,
            FacultyDegree.AssociateProfessor
        );

        if (facultyResult.IsSuccess)
        {
            await unitOfWork.FacultyRepository.AddAsync(facultyResult.Value);
            await unitOfWork.SaveChangesAsync();
        }
    }
}