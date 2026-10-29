using CaseManagementAudit.Common;

namespace CaseManagementAudit.Entities;

public class CaseRecord : IAuditableEntity
{
    public Guid Id { get; set; }
    public string CaseNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CaseStatus Status { get; set; } = CaseStatus.Submitted;
    public CasePriority Priority { get; set; } = CasePriority.Medium;

    public byte[]? RowVersion { get; set; }

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedUtc { get; set; }

    public List<CaseNote> Notes { get; set; } = new();
    public List<Document> Documents { get; set; } = new();
    public List<CaseAssignee> CaseAssignees { get; set; } = new();
}