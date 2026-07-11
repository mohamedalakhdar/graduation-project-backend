using CollegeControlSystem.Domain.Courses;
using CollegeControlSystem.Domain.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollegeControlSystem.Infrastructure.Configurations
{
    internal sealed class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            // 1. Table Name
            builder.ToTable("Courses");

            // 2. Primary Key
            builder.HasKey(c => c.Id);

            // 3. Value Object Mapping (CourseCode)
            // We convert the Value Object to a simple string for the DB column.
            builder.Property(c => c.Code)
                .HasConversion(
                    code => code.Value,                      // To DB: Extract string
                    value => CourseCode.Create(value).Value  // From DB: Recreate Object (Assuming DB data is valid)
                )
                .HasColumnName("Code")
                .HasMaxLength(10)
                .IsRequired();

            // 4. Standard Properties
            builder.Property(c => c.Title)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.Property(c => c.Credits).IsRequired();
            builder.Property(c => c.LectureHours).IsRequired();
            builder.Property(c => c.LabHours).IsRequired();

            // 5. Relationships

            // Department (Many-to-One)
            builder.HasOne<Department>()
                .WithMany() // Assuming Department doesn't have a specific 'Courses' navigation list exposed
                .HasForeignKey(c => c.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prerequisites (One-to-Many to the Join Table)
            builder.HasMany(c => c.Prerequisites)
                .WithOne(cp => cp.Course)
                .HasForeignKey(cp => cp.CourseId)
                .OnDelete(DeleteBehavior.Cascade); // If Course is deleted, its prereq links are deleted

            // 6. Indexes
            // Ensure Course Code is unique (e.g., CCE 101 can only appear once)
            builder.HasIndex(c => c.Code).IsUnique();
        }
    }
}
