using CollegeControlSystem.Domain.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace CollegeControlSystem.Infrastructure.Configurations
{
    internal sealed class ProgramConfiguration : IEntityTypeConfiguration<Program>
    {
        public void Configure(EntityTypeBuilder<Program> builder)
        {
            // 1. Table Name
            builder.ToTable("Programs");

            // 2. Primary Key
            builder.HasKey(p => p.Id);

            // 3. Properties
            builder.Property(p => p.Name)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(p => p.RequiredCredits)
                .IsRequired();// Required for graduation audit [cite: 145]

            // 4. Relationships
            // Already configured in DepartmentConfiguration, but we can reinforce the FK here.
            builder.HasOne(p => p.Department)
                .WithMany(d => d.Programs)
                .HasForeignKey(p => p.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict); ;

            // 5. Indexes
            // Program names should be unique per Department
            builder.HasIndex(p => new { p.DepartmentId, p.Name }).IsUnique();
        }
    }
}
