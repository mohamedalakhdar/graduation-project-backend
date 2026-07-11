using CollegeControlSystem.Domain.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace CollegeControlSystem.Infrastructure.Configurations
{
    internal sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            // 1. Table Name
            builder.ToTable("Departments");

            // 2. Primary Key
            builder.HasKey(d => d.Id);

            // 3. Properties
            builder.Property(d => d.DepartmentName)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(d => d.Description)
                .HasMaxLength(500)
                .IsRequired(false);

            // 4. Relationships
            // A Department has many Programs.
            // We configure the relationship here to ensure the navigation property is handled correctly.
            //builder.HasMany(d => d.Programs)
            //    .WithOne(p => p.Department)
            //    .HasForeignKey(p => p.DepartmentId)
            //    // Rule: "Cannot delete a Department if it has linked Programs" [cite: 195, 308]
            //                // Therefore, we use Restrict instead of Cascade.
            //    .OnDelete(DeleteBehavior.Restrict);

            // 5. Indexes
            builder.HasIndex(d => d.DepartmentName).IsUnique();
        }
    }
}
