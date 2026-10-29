using CaseManagementAudit.Common;

namespace CaseManagementAudit.Entities;

public class Document : IAuditableEntity
{
    public Guid Id { get; set; }
    public Guid CaseRecordId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = "application/octet-stream";
    public long FileSizeBytes { get; set; }
    public string StorageLocation { get; set; } = string.Empty;

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedUtc { get; set; }

    public CaseRecord? CaseRecord { get; set; }
}