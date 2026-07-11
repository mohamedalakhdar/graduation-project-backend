using CollegeControlSystem.Application;
using CollegeControlSystem.Infrastructure;
namespace CollegeControlSystem.Presentation.Extenstions
{
    public  static class DependencyInjectionService
    {
        public static IServiceCollection AddDependencyInjectionService(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddApplicationLayer();
            services.AddInfrastructureLayer(configuration);

            return services;
        }
    }
}
