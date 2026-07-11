using CollegeControlSystem.Infrastructure.Seeders;

namespace CollegeControlSystem.Presentation.Extenstions
{
    public static class DatabaseSeedExtension
    {
        public static async Task SeedDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            await RoleSeeder.SeedAsync(services);
            await AdminSeeder.SeedAsync(services);
            await AcademicSeeder.SeedAsync(services);
            await UserSeeder.SeedAsync(services);
        }
    }
}
