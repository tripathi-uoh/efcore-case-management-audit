using CaseManagementAudit.Entities;
using Microsoft.EntityFrameworkCore;

namespace CaseManagementAudit.Data;

public class CaseDbContext : DbContext
{
    public CaseDbContext(DbContextOptions<CaseDbContext> options) : base(options)
    {
    }

    public DbSet<CaseRecord> CaseRecords => Set<CaseRecord>();
    public DbSet<CaseNote> CaseNotes => Set<CaseNote>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<Assignee> Assignees => Set<Assignee>();
    public DbSet<CaseAssignee> CaseAssignees => Set<CaseAssignee>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CaseDbContext).Assembly);
    }
}