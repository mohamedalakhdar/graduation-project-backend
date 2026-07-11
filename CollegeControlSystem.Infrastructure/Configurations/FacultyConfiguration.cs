using CollegeControlSystem.Domain.Faculties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollegeControlSystem.Infrastructure.Configurations
{
    internal sealed class FacultyConfiguration : IEntityTypeConfiguration<Faculty>
    {
        public void Configure(EntityTypeBuilder<Faculty> builder)
        {
            // 1. Table Name
            builder.ToTable("Faculties");

            // 2. Primary Key
            builder.HasKey(f => f.Id);

            // 3. Properties
            builder.Property(f => f.FacultyName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(f => f.Degree)
                .HasConversion<string>() // Store enum as string for readability
                .HasMaxLength(50) // e.g. "PhD", "Professor"
                .IsRequired();

            builder.Property(f => f.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasDefaultValue(FacultyStatus.Active);

            // 4. Relationships

            // Link to Identity User (One-to-One)
            builder.HasOne(f => f.AppUser)
                .WithOne()
                .HasForeignKey<Faculty>(f => f.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Link to Department (Many-to-One)
            builder.HasOne(f => f.Department)
                .WithMany() // Assuming Department has no specific 'Faculties' list
                .HasForeignKey(f => f.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting Dept if it has Faculty

            // Indexes for performance
            builder.HasIndex(f => f.DepartmentId);
            builder.HasIndex(f => f.AppUserId).IsUnique();
        }
    }
}
