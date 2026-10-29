namespace CaseManagementAudit.Entities;

public class CaseAssignee
{
    public Guid CaseRecordId { get; set; }
    public CaseRecord CaseRecord { get; set; } = null!;

    public Guid AssigneeId { get; set; }
    public Assignee Assignee { get; set; } = null!;

    public DateTime AssignedDateUtc { get; set; } = DateTime.UtcNow;
    public string RoleOnCase { get; set; } = "Investigator";
}