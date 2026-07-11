using Microsoft.Extensions.Configuration;

namespace ByteStore.Api.Extenstions
{
    public static class HealthCheckService
    {
        public static IServiceCollection AddHealthCheck(this IServiceCollection services,IConfiguration configuration)
        {
            // health check
            var connectionString = configuration.GetConnectionString("DefaultConnection")
               ?? throw new ArgumentNullException(nameof(configuration));
            //var redisConnection = configuration.GetConnectionString("Redis")
            //   ?? throw new ArgumentNullException(nameof(configuration));

            services.AddHealthChecks()
                .AddNpgSql(connectionString, name: "postgresql", tags: new[] { "db", "sql" }); // Postgres
                //.AddRedis(redisConnection, name: "redis", tags: new[] { "cache", "nosql" });  // Redis

            // it cause problem of more than dbContext was found
            ////health check with Dashboard
            services.AddHealthChecksUI(options =>
            {
                //options.SetEvaluationTimeInSeconds(15); // كل 15 ثانية يعمل Check
                //options.MaximumHistoryEntriesPerEndpoint(60); // يحتفظ بالـ History لآخر 60 نتيجة
                options.AddHealthCheckEndpoint("CollegeControlSystem", "/health"); // بيراقب الـ API نفسه
            }).AddInMemoryStorage();

            return services;
        }
    }
}
