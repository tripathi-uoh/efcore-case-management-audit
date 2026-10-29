using CaseManagementAudit.Entities;
using Microsoft.EntityFrameworkCore;

namespace CaseManagementAudit.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(CaseDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (await context.CaseRecords.AnyAsync())
        {
            return;
        }

        var investigator = new Assignee
        {
            Id = Guid.NewGuid(),
            FullName = "Alex Mercer",
            Email = "alex.mercer@enterprise.internal",
            Department = "Fraud Investigations"
        };

        var leadAuditor = new Assignee
        {
            Id = Guid.NewGuid(),
            FullName = "Elena Rostova",
            Email = "elena.rostova@enterprise.internal",
            Department = "Internal Compliance"
        };

        context.Assignees.AddRange(investigator, leadAuditor);

        var case1 = new CaseRecord
        {
            Id = Guid.NewGuid(),
            CaseNumber = "CASE-2026-001",
            Title = "Unauthorized Corporate Wire Transfer Investigation",
            Description = "Anomalous fund transfer detected originating from regional accounts.",
            Status = CaseStatus.UnderInvestigation,
            Priority = CasePriority.Critical,
            CreatedUtc = DateTime.UtcNow.AddDays(-14)
        };

        case1.Notes.Add(new CaseNote
        {
            Id = Guid.NewGuid(),
            Author = "Alex Mercer",
            Content = "Initial transaction logs requested from core banking database.",
            CreatedUtc = DateTime.UtcNow.AddDays(-13)
        });

        case1.Notes.Add(new CaseNote
        {
            Id = Guid.NewGuid(),
            Author = "Alex Mercer",
            Content = "Subpoena issued for intermediary correspondent bank routing records.",
            CreatedUtc = DateTime.UtcNow.AddDays(-10)
        });

        case1.Documents.Add(new Document
        {
            Id = Guid.NewGuid(),
            FileName = "wire_ledger_extract.csv",
            ContentType = "text/csv",
            FileSizeBytes = 458920,
            StorageLocation = "s3://compliance-vault/2026/001/wire_ledger.csv",
            CreatedUtc = DateTime.UtcNow.AddDays(-12)
        });

        case1.CaseAssignees.Add(new CaseAssignee
        {
            CaseRecord = case1,
            Assignee = investigator,
            RoleOnCase = "Lead Investigator",
            AssignedDateUtc = DateTime.UtcNow.AddDays(-14)
        });

        var case2 = new CaseRecord
        {
            Id = Guid.NewGuid(),
            CaseNumber = "CASE-2026-002",
            Title = "Third-Party Vendor SOC2 Compliance Breach",
            Description = "Audit findings reveal unencrypted backup archives at cloud partner.",
            Status = CaseStatus.PendingReview,
            Priority = CasePriority.High,
            CreatedUtc = DateTime.UtcNow.AddDays(-30)
        };

        case2.Notes.Add(new CaseNote
        {
            Id = Guid.NewGuid(),
            Author = "Elena Rostova",
            Content = "Remediation plan submitted by vendor; pending penetration test validation.",
            CreatedUtc = DateTime.UtcNow.AddDays(-5)
        });

        var case3 = new CaseRecord
        {
            Id = Guid.NewGuid(),
            CaseNumber = "CASE-2025-089",
            Title = "Legacy Database Migration Reconciliation",
            Description = "Historical discrepancies during migration from legacy SQL Server to PostgreSQL.",
            Status = CaseStatus.Resolved,
            Priority = CasePriority.Low,
            CreatedUtc = DateTime.UtcNow.AddYears(-1).AddDays(-30)
        };

        context.CaseRecords.AddRange(case1, case2, case3);
        await context.SaveChangesAsync();
    }
}