using CollegeControlSystem.Domain.CourseOfferings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace CollegeControlSystem.Infrastructure.Configurations
{
    internal sealed class CourseOfferingConfiguration : IEntityTypeConfiguration<CourseOffering>
    {
        public void Configure(EntityTypeBuilder<CourseOffering> builder)
        {
            // 1. Table Name
            builder.ToTable("CourseOfferings");

            // 2. Primary Key
            builder.HasKey(c => c.Id);

            // 3. Properties
            builder.Property(c => c.Capacity)
                .IsRequired();

            builder.Property(c => c.CurrentEnrolled)
                .HasDefaultValue(0)
                .IsConcurrencyToken(); // Optional: Helps handle race conditions during registration

            // 4. Value Object Mapping (Semester)
            // This maps 'Semester' properties to columns 'Term' and 'Year' in the same table.
            builder.OwnsOne(c => c.Semester, semester =>
            {
                semester.Property(s => s.Term)
                    .HasColumnName("Term")       // DB Column Name
                    .HasConversion<string>()     // Store Enum as String (Fall, Spring)
                    .IsRequired();

                semester.Property(s => s.Year)
                    .HasColumnName("Year")
                    .IsRequired();

                // Index for faster searching by Semester
                semester.HasIndex(s => new { s.Year, s.Term });
            });

            // 5. Relationships

            // Link to Course (Many-to-One)
            builder.HasOne(co => co.Course)
                .WithMany() // Assuming Course doesn't hold a list of historical offerings
                .HasForeignKey(co => co.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Link to Instructor (Many-to-One)
            builder.HasOne(co => co.Instructor)
                .WithMany() // Assuming Faculty doesn't hold a list of offerings
                .HasForeignKey(co => co.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
