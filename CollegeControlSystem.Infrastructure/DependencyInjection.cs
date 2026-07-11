using CollegeControlSystem.Application.Abstractions.IService;
using CollegeControlSystem.Application.Identity;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.CourseOfferings;
using CollegeControlSystem.Domain.Courses;
using CollegeControlSystem.Domain.Departments;
using CollegeControlSystem.Domain.Faculties;
using CollegeControlSystem.Domain.Identity;
using CollegeControlSystem.Domain.RegistrationPeriods;
using CollegeControlSystem.Domain.Registrations;
using CollegeControlSystem.Domain.Students;
using CollegeControlSystem.Infrastructure.Database;
using CollegeControlSystem.Infrastructure.Helpers;
using CollegeControlSystem.Infrastructure.Integration;
using CollegeControlSystem.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CollegeControlSystem.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
        {

            services.Configure<AdminAccount>(configuration.GetSection("AdminAccount"));

            //services.Configure<ImageKitOptions>(configuration.GetSection("ImageKitOptions"));

            // Configure brevo for email 
            services.Configure<Brevo>(configuration.GetSection("Brevo"));

            services.Configure<AdminAccount>(configuration.GetSection("AdminAccount"));


            #region EFCore
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(configuration));
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
            // shortcut
            //services.AddNpgsql<AppDbContext>((configuration.GetConnectionString("DefaultConnection")));
            #endregion

            //#region Redis
            //var redisConnection = configuration.GetConnectionString("Redis")
            //     ?? throw new ArgumentNullException(nameof(configuration));
            //services.AddSingleton<IConnectionMultiplexer>((serviceProvider) =>
            //{
            //    return ConnectionMultiplexer.Connect(redisConnection);
            //});
            //#endregion

            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IEmailService, EmailService>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<IGradeRepository, GradeRepository>();
            services.AddScoped<ICourseOfferingRepository, CourseOfferingRepository>();
            services.AddScoped<IFacultyRepository, FacultyRepository>();
            services.AddScoped<IRegistrationRepository, RegistrationRepository>();
            services.AddScoped<IRegistrationPeriodRepository, RegistrationPeriodRepository>();
            

            #region JWTConfigs
            //(1)
            // identity ===> i spend one day to find out that you are the problem => it should be above JWTConfigs :(
            services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();


            // JWTHelper (2)
            services.Configure<JWT>(configuration.GetSection("JWT"));

            // (3)
            // to use jwt token to check authantication =>[authorize]
            services.AddAuthentication(options =>
            {
                // to change default authantication to jwt 
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

                //  if u are unauthanticated it will redirect you to login form
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                // if there other schemas make is default of jwt
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;


                // these configs to check if has token only but i want to check if he has right claims
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;

                // check if token have specific data
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidAudience = configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"])),

                    // if u want when the token expire he does not give me مهله بعض الوقت 
                    ClockSkew = TimeSpan.Zero

                };
            }

                         );
            #endregion



            return services;
        }
    }

}