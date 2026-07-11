
using Prometheus;
using ByteStore.Api.Extenstions;
using CollegeControlSystem.Infrastructure.Seeders;
using CollegeControlSystem.Presentation.Extenstions;
using CollegeControlSystem.Presentation.Middlewares;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

namespace CollegeControlSystem.Presentation
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //serilog;
            builder.Host.UseSerilog((context, loggerConfig) =>
                loggerConfig.ReadFrom.Configuration(context.Configuration));

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Configuration.AddJsonFile("Secret.json", optional: false, reloadOnChange: true);
            builder.Services.AddDependencyInjectionService(builder.Configuration);
            builder.Services.AddRateLimiting();
            builder.Services.AddHealthCheck(builder.Configuration);

            builder.Services.AddSwaggerDocumentation();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
                //app.MapOpenApi();
                app.UseSwaggerDocumentation();
            //}
            app.UseRateLimiter();
            app.UseCustomCors();
            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();

            // custom middlewares
            app.UseCustomExceptionHandler();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHttpMetrics();

            app.MapControllers();

            // seeding
            //using (var scope = app.Services.CreateScope())
            //{
            //    var services = scope.ServiceProvider;

            //    await RoleSeeder.SeedAsync(services);
            //    await AdminSeeder.SeedAsync(services);
            //}
            await app.SeedDatabaseAsync();

            // it cause problem of more than dbContext was found
            //  Map Health Checks JSON Endpoint => normal health check just api return json response
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                //This formats the response in a special JSON format that the HealthCheckUI dashboard understands.
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });


            // Map HealthCheck UI Dashboard
            app.MapHealthChecksUI(options =>
            {
                options.UIPath = "/health-ui"; // Dashboard path
                options.ApiPath = "/health-ui-api"; // API used by the UI
            });

            app.MapMetrics();

            app.Run();
        }
    }
}
