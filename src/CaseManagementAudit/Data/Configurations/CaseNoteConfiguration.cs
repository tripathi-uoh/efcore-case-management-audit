using CaseManagementAudit.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CaseManagementAudit.Data.Configurations;

public class CaseNoteConfiguration : IEntityTypeConfiguration<CaseNote>
{
    public void Configure(EntityTypeBuilder<CaseNote> builder)
    {
        builder.ToTable("CaseNotes");
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Author).IsRequired().HasMaxLength(128);
        builder.Property(n => n.Content).IsRequired().HasMaxLength(4000);
        builder.HasIndex(n => n.CaseRecordId);
    }
}