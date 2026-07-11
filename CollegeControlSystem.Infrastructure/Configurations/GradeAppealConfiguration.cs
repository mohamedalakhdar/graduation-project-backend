using CollegeControlSystem.Domain.Registrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollegeControlSystem.Infrastructure.Configurations;

internal sealed class GradeAppealConfiguration : IEntityTypeConfiguration<GradeAppeal>
{
    public void Configure(EntityTypeBuilder<GradeAppeal> builder)
    {
        builder.ToTable("GradeAppeals");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.AppealReason)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(a => a.AppealDate)
            .IsRequired();

        builder.Property(a => a.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(a => a.ReviewNotes)
            .HasMaxLength(2000);

        builder.Property(a => a.ReviewedDate);

        builder.HasOne(a => a.Grade)
            .WithMany()
            .HasForeignKey(a => a.GradeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => a.GradeId);
    }
}
