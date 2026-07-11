using CollegeControlSystem.Domain.Faculties;
using CollegeControlSystem.Domain.Students;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace CollegeControlSystem.Infrastructure.Configurations
{
    internal sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            // 1. Table Name
            builder.ToTable("Students");

            // 2. Primary Key
            builder.HasKey(s => s.Id);

            // 3. Properties Configuration
            builder.Property(s => s.StudentName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(s => s.AcademicNumber)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(s => s.NationalId)
                .HasMaxLength(14)
                .IsFixedLength() // Enforce strictly 14 chars
                .IsRequired();

            // 4. Enums Conversion
            // Storing as Strings makes the DB more readable ('AcademicWarning' vs '2')
            builder.Property(s => s.AcademicStatus)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(s => s.AcademicLevel)
                .HasConversion<string>()
                .HasMaxLength(50);

            // 5. Decimal Precision for GPA (e.g., 3.85)
            builder.Property(s => s.CGPA)
                .HasPrecision(3, 2)
                .HasDefaultValue(0.00m);

            // 6. Indexes
            builder.HasIndex(s => s.AcademicNumber).IsUnique();
            builder.HasIndex(s => s.NationalId).IsUnique();
            builder.HasIndex(s => s.AppUserId).IsUnique(); // One User = One Student

            // 7. Relationships

            // Link to Program
            builder.HasOne(s => s.Program)
                .WithMany() // Assuming Program doesn't have a 'Students' list, or generic
                .HasForeignKey(s => s.ProgramId)
                .OnDelete(DeleteBehavior.Restrict); // Don't delete student if Program is deleted

            // Link to Identity User
            builder.HasOne(s => s.AppUser)
                .WithOne() // One-to-One
                .HasForeignKey<Student>(s => s.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Link to Advisor (Faculty)
            // Note: Student entity has 'AdvisorId' but no 'Advisor' Nav Property in your snippet.
            // We can still enforce the FK constraint:
            builder.HasOne<Faculty>()
                .WithMany()
                .HasForeignKey(s => s.AdvisorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Link to Registrations
            builder.HasMany(s => s.Registrations)
                .WithOne(r => r.Student) // Assuming Registration has 'Student' prop
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}