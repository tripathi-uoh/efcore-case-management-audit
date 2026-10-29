using CaseManagementAudit.Data;
using CaseManagementAudit.Entities;
using Microsoft.EntityFrameworkCore;

namespace CaseManagementAudit.Endpoints;

public static class CaseEndpoints
{
    public static RouteGroupBuilder MapCaseEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/cases")
            .WithTags("Cases & Queries");

        group.MapGet("/", async (CaseDbContext db, CaseStatus? status, int page = 1, int pageSize = 10) =>
        {
            var query = db.CaseRecords.AsNoTracking();

            if (status.HasValue)
            {
                query = query.Where(c => c.Status == status.Value);
            }

            var totalCount = await query.CountAsync();

            var cases = await query
                .OrderByDescending(c => c.CreatedUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new
                {
                    c.Id,
                    c.CaseNumber,
                    c.Title,
                    Status = c.Status.ToString(),
                    Priority = c.Priority.ToString(),
                    c.CreatedUtc,
                    NoteCount = c.Notes.Count,
                    DocumentCount = c.Documents.Count
                })
                .ToListAsync();

            return Results.Ok(new { TotalCount = totalCount, Page = page, PageSize = pageSize, Items = cases });
        })
        .WithName("GetCasesPaged")
        .WithSummary("Query cases with AsNoTracking and projection");

        group.MapGet("/{id:guid}", async (CaseDbContext db, Guid id, bool splitQuery = true) =>
        {
            var baseQuery = db.CaseRecords
                .Where(c => c.Id == id)
                .Include(c => c.Notes)
                .Include(c => c.Documents)
                .Include(c => c.CaseAssignees)
                    .ThenInclude(ca => ca.Assignee)
                .AsNoTracking();

            if (splitQuery)
            {
                baseQuery = baseQuery.AsSplitQuery();
            }

            var caseRecord = await baseQuery.FirstOrDefaultAsync();

            if (caseRecord is null)
            {
                return Results.NotFound(new { Message = $"Case with ID {id} was not found." });
            }

            return Results.Ok(new
            {
                caseRecord.Id,
                caseRecord.CaseNumber,
                caseRecord.Title,
                caseRecord.Description,
                Status = caseRecord.Status.ToString(),
                Priority = caseRecord.Priority.ToString(),
                caseRecord.CreatedUtc,
                caseRecord.UpdatedUtc,
                Notes = caseRecord.Notes.Select(n => new { n.Id, n.Author, n.Content, n.CreatedUtc }),
                Documents = caseRecord.Documents.Select(d => new { d.Id, d.FileName, d.FileSizeBytes, d.StorageLocation }),
                Assignees = caseRecord.CaseAssignees.Select(ca => new { ca.Assignee.FullName, ca.Assignee.Email, ca.RoleOnCase }),
                QueryModeUsed = splitQuery ? "SplitQuery (Multiple optimized SELECT statements)" : "SingleQuery (Cartesian JOIN)"
            });
        })
        .WithName("GetCaseById")
        .WithSummary("Load case aggregate using AsSplitQuery() vs SingleQuery");

        group.MapPost("/", async (CaseDbContext db, CreateCaseRequest request) =>
        {
            var caseRecord = new CaseRecord
            {
                Id = Guid.NewGuid(),
                CaseNumber = $"CASE-{DateTime.UtcNow.Year}-{new Random().Next(100, 999)}",
                Title = request.Title,
                Description = request.Description,
                Status = request.Status,
                Priority = request.Priority
            };

            db.CaseRecords.Add(caseRecord);
            await db.SaveChangesAsync();

            return Results.Created($"/api/cases/{caseRecord.Id}", caseRecord);
        })
        .WithName("CreateCase")
        .WithSummary("Creates a new case; automatically audited via Interceptor");

        group.MapPut("/{id:guid}", async (CaseDbContext db, Guid id, UpdateCaseRequest request) =>
        {
            var caseRecord = await db.CaseRecords.FindAsync(id);
            if (caseRecord is null)
            {
                return Results.NotFound(new { Message = "Case not found." });
            }

            caseRecord.Title = request.Title;
            caseRecord.Description = request.Description;
            caseRecord.Status = request.Status;
            caseRecord.Priority = request.Priority;

            await db.SaveChangesAsync();

            return Results.Ok(new { Message = "Case updated successfully.", Case = caseRecord });
        })
        .WithName("UpdateCase")
        .WithSummary("Updates a case; Interceptor calculates diff and appends to AuditLogs");

        group.MapPost("/bulk-archive-resolved", async (CaseDbContext db, int olderThanDays = 30) =>
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-olderThanDays);

            var affectedRows = await db.CaseRecords
                .Where(c => c.Status == CaseStatus.Resolved && c.CreatedUtc < cutoffDate)
                .ExecuteUpdateAsync(setter => setter
                    .SetProperty(c => c.Status, CaseStatus.Archived)
                    .SetProperty(c => c.UpdatedUtc, DateTime.UtcNow));

            return Results.Ok(new
            {
                Message = "Bulk archive completed without pulling records into memory.",
                AffectedRows = affectedRows,
                OlderThanCutoff = cutoffDate
            });
        })
        .WithName("BulkArchiveCases")
        .WithSummary("EF Core Bulk mutation via ExecuteUpdateAsync");

        routes.MapGet("/api/audits", async (CaseDbContext db, string? entityName, int limit = 20) =>
        {
            var query = db.AuditLogs
                .AsNoTracking()
                .OrderByDescending(a => a.TimestampUtc)
                .AsQueryable();

            if (!string.IsNullOrEmpty(entityName))
            {
                query = query.Where(a => a.EntityName == entityName);
            }

            var logs = await query.Take(limit).ToListAsync();
            return Results.Ok(logs);
        })
        .WithTags("Audit Logs")
        .WithSummary("Inspect the audit trail created by AuditSaveChangesInterceptor");

        return group;
    }
}

public record CreateCaseRequest(string Title, string Description, CaseStatus Status, CasePriority Priority);
public record UpdateCaseRequest(string Title, string Description, CaseStatus Status, CasePriority Priority);