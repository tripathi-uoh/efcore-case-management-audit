using CaseManagementAudit.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CaseManagementAudit.Data.Configurations;

public class CaseAssigneeConfiguration : IEntityTypeConfiguration<CaseAssignee>
{
    public void Configure(EntityTypeBuilder<CaseAssignee> builder)
    {
        builder.ToTable("CaseAssignees");
        builder.HasKey(ca => new { ca.CaseRecordId, ca.AssigneeId });
        builder.Property(ca => ca.RoleOnCase).IsRequired().HasMaxLength(64);

        builder.HasOne(ca => ca.CaseRecord)
            .WithMany(c => c.CaseAssignees)
            .HasForeignKey(ca => ca.CaseRecordId);

        builder.HasOne(ca => ca.Assignee)
            .WithMany(a => a.CaseAssignees)
            .HasForeignKey(ca => ca.AssigneeId);
    }
}