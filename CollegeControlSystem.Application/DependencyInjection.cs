using CollegeControlSystem.Application.Abstractions.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CollegeControlSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));

            //configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // includeInternalTypes for swagger or other reflection-based libs
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly,
         includeInternalTypes: true);

        return services;
    }
}