using CollegeControlSystem.Domain.Registrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollegeControlSystem.Infrastructure.Configurations
{
    internal sealed class RegistrationConfiguration : IEntityTypeConfiguration<Registration>
    {
        public void Configure(EntityTypeBuilder<Registration> builder)
        {
            // 1. Table Name
            builder.ToTable("Registrations");

            // 2. Primary Key
            builder.HasKey(r => r.Id);

            // 3. Properties
            builder.Property(r => r.Status)
                .HasConversion<string>() // Store Enum as String ('Pending', 'Approved')
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(r => r.IsRetake)
                .IsRequired();

            builder.Property(r => r.RegistrationDate)
                .IsRequired();

            // 4. Relationships

            // Link to Student (Many-to-One)
            builder.HasOne(r => r.Student)
                .WithMany(s => s.Registrations)
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.Restrict); // Don't delete student if they have registrations (History)

            // Link to CourseOffering (Many-to-One)
            builder.HasOne(r => r.CourseOffering)
                .WithMany() // Assuming Offering doesn't hold a list of registrations directly or using generic
                .HasForeignKey(r => r.CourseOfferingId)
                .OnDelete(DeleteBehavior.Restrict);

            // Link to Grade (One-to-One)
            // A Registration has one Grade (optional until graded)
            builder.HasOne(r => r.Grade)
                .WithOne(g => g.Registration)
                .HasForeignKey<Grade>(g => g.RegistrationId)
                .OnDelete(DeleteBehavior.Cascade); // If Registration is deleted, Grade goes with it.

            // 5. Indexes
            // Ensure a student can't register for the same offering twice
            builder.HasIndex(r => new { r.StudentId, r.CourseOfferingId }).IsUnique();

            // Index for performance on Advisor queries
            builder.HasIndex(r => r.Status);
        }
    }
}
