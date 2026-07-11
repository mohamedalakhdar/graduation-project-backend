using CollegeControlSystem.Domain.Registrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollegeControlSystem.Infrastructure.Configurations
{
    internal sealed class GradeConfiguration : IEntityTypeConfiguration<Grade>
    {
        public void Configure(EntityTypeBuilder<Grade> builder)
        {
            // 1. Table Name
            builder.ToTable("Grades");

            // 2. Primary Key
            builder.HasKey(g => g.Id);

            // 3. Properties
            builder.Property(g => g.SemesterWorkGrade)
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            builder.Property(g => g.FinalGrade)
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            builder.Property(g => g.TotalGrade)
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            builder.Property(g => g.GradePoints)
                .HasColumnType("decimal(3,2)") // e.g., 4.0, 3.7
                .IsRequired();

            builder.Property(g => g.LetterGrade)
                .HasMaxLength(3) // "A+", "B-"
                .IsRequired();

            // 4. Relationships
            // Configured in RegistrationConfiguration (HasOneWithOne), but reinforced here:
            builder.HasOne(g => g.Registration)
                .WithOne(r => r.Grade)
                .HasForeignKey<Grade>(g => g.RegistrationId);
        }
    }
}
