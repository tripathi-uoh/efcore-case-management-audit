namespace CaseManagementAudit.Common;

/// <summary>
/// Marker interface indicating an entity should automatically be audited by AuditSaveChangesInterceptor.
/// </summary>
public interface IAuditableEntity
{
    Guid Id { get; }
    DateTime CreatedUtc { get; set; }
    DateTime? UpdatedUtc { get; set; }
}