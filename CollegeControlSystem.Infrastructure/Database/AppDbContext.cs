using CollegeControlSystem.Application.Abstractions.Exceptions;
using CollegeControlSystem.Domain.Abstractions;
using CollegeControlSystem.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace CollegeControlSystem.Infrastructure.Database
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        private readonly IPublisher publisher;
        public AppDbContext(DbContextOptions<AppDbContext> options, IPublisher publisher) : base(options)
        {
            this.publisher = publisher;
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>().ToTable("Users");
            builder.Entity<AppRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            try
            {
                // Get all tracked entities that have domain events
                var domainEntities = ChangeTracker
                    .Entries<Entity>()
                    .Where(entry => entry.Entity.GetDomainEvents().Any())
                    .Select(entry => entry.Entity)
                    .ToList();

                // Copy the events to avoid modification during iteration
                var domainEvents = domainEntities
                    .SelectMany(entity => entity.GetDomainEvents())
                    .ToList();

                //foreach (var entry in ChangeTracker.Entries())
                //{
                //    Console.WriteLine($"{entry.Entity.GetType().Name} - {entry.State}");
                //}


                // Save changes first
                var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

                // Dispatch events only if save was successful
                foreach (var domainEvent in domainEvents)
                {
                    await publisher.Publish(domainEvent, cancellationToken);
                }

                // Clear events after dispatch
                domainEntities.ForEach(entity => entity.ClearDomainEvents());

                return result;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ConcurrencyException("A concurrency error occurred while saving changes.", ex);
            }
        }
    
    }
}
