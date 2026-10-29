using CaseManagementAudit.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CaseManagementAudit.Data.Configurations;

public class CaseRecordConfiguration : IEntityTypeConfiguration<CaseRecord>
{
    public void Configure(EntityTypeBuilder<CaseRecord> builder)
    {
        builder.ToTable("CaseRecords");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.CaseNumber).IsRequired().HasMaxLength(32);
        builder.HasIndex(c => c.CaseNumber).IsUnique();

        builder.Property(c => c.Title).IsRequired().HasMaxLength(256);
        builder.Property(c => c.Description).HasMaxLength(2000);

        builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(32);
        builder.Property(c => c.Priority).HasConversion<string>().HasMaxLength(32);

        builder.HasIndex(c => new { c.Status, c.Priority });
        builder.Property(c => c.RowVersion).IsRowVersion();

        builder.HasMany(c => c.Notes)
            .WithOne(n => n.CaseRecord)
            .HasForeignKey(n => n.CaseRecordId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Documents)
            .WithOne(d => d.CaseRecord)
            .HasForeignKey(d => d.CaseRecordId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}