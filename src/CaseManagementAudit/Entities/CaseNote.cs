using CaseManagementAudit.Common;

namespace CaseManagementAudit.Entities;

public class CaseNote : IAuditableEntity
{
    public Guid Id { get; set; }
    public Guid CaseRecordId { get; set; }
    public string Author { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedUtc { get; set; }

    public CaseRecord? CaseRecord { get; set; }
}