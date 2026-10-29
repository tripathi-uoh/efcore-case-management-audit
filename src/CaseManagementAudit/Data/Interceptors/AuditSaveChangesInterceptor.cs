using System.Text.Json;
using CaseManagementAudit.Common;
using CaseManagementAudit.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CaseManagementAudit.Data.Interceptors;

public class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        GenerateAuditEntries(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        GenerateAuditEntries(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void GenerateAuditEntries(DbContext? context)
    {
        if (context is null) return;

        var entries = context.ChangeTracker.Entries<IAuditableEntity>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        if (!entries.Any()) return;

        var now = DateTime.UtcNow;
        var auditLogs = new List<AuditLog>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedUtc = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedUtc = now;
            }

            var diff = new Dictionary<string, object?>();
            foreach (var prop in entry.Properties)
            {
                if (prop.Metadata.IsPrimaryKey()) continue;

                if (entry.State == EntityState.Added)
                {
                    diff[prop.Metadata.Name] = prop.CurrentValue;
                }
                else if (entry.State == EntityState.Modified && prop.IsModified)
                {
                    diff[prop.Metadata.Name] = new
                    {
                        Original = prop.OriginalValue,
                        Current = prop.CurrentValue
                    };
                }
                else if (entry.State == EntityState.Deleted)
                {
                    diff[prop.Metadata.Name] = prop.OriginalValue;
                }
            }

            var audit = new AuditLog
            {
                EntityName = entry.Entity.GetType().Name,
                EntityId = entry.Entity.Id.ToString(),
                Action = entry.State.ToString(),
                ChangesJson = JsonSerializer.Serialize(diff),
                TimestampUtc = now,
                PerformedBy = "DevUser"
            };

            auditLogs.Add(audit);
        }

        context.Set<AuditLog>().AddRange(auditLogs);
    }
}