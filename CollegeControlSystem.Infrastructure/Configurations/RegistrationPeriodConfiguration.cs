using CollegeControlSystem.Domain.RegistrationPeriods;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollegeControlSystem.Infrastructure.Configurations
{
    internal sealed class RegistrationPeriodConfiguration : IEntityTypeConfiguration<RegistrationPeriod>
    {
        public void Configure(EntityTypeBuilder<RegistrationPeriod> builder)
        {
            builder.ToTable("RegistrationPeriods");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(p => p.StartDateUtc)
                .IsRequired();

            builder.Property(p => p.EndDateUtc)
                .IsRequired();

            builder.Property(p => p.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(p => p.CreatedAtUtc)
                .IsRequired();

            builder.OwnsOne(p => p.Semester, semester =>
            {
                semester.Property(s => s.Term)
                    .HasColumnName("Term")
                    .HasConversion<string>()
                    .IsRequired();

                semester.Property(s => s.Year)
                    .HasColumnName("Year")
                    .IsRequired();

                semester.HasIndex(s => new { s.Year, s.Term });
            });

            builder.HasIndex(p => p.IsActive);
        }
    }
}
