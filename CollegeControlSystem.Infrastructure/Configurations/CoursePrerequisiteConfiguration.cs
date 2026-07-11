using CollegeControlSystem.Domain.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollegeControlSystem.Infrastructure.Configurations
{
    internal sealed class CoursePrerequisiteConfiguration : IEntityTypeConfiguration<CoursePrerequisite>
    {
        public void Configure(EntityTypeBuilder<CoursePrerequisite> builder)
        {
            // 1. Table Name
            builder.ToTable("CoursePrerequisites");

            // 2. Composite Primary Key
            //builder.HasKey(cp => new { cp.CourseId, cp.PrerequisiteCourseId });
            builder.HasKey(cp => cp.Id);

            // 3. Relationships

            // The "Parent" Course (The one that requires prerequisites)
            // Configured in CourseConfiguration, but reinforced here:
            builder.HasOne(cp => cp.Course)
                .WithMany(c => c.Prerequisites)
                .HasForeignKey(cp => cp.CourseId)
                .OnDelete(DeleteBehavior.Cascade); // Deleting the course deletes its requirements list

            // The "Child" Course (The requirement itself)
            builder.HasOne(cp => cp.PrerequisiteCourse)
                .WithMany() // We don't have a 'DependentCourses' list on Course
                .HasForeignKey(cp => cp.PrerequisiteCourseId)
                .OnDelete(DeleteBehavior.Restrict); // Important: Deleting a course that is a prerequisite for others should be blocked or restricted!
        }
    }
}
