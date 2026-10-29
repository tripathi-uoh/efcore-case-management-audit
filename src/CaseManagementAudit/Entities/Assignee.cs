namespace CaseManagementAudit.Entities;

public class Assignee
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;

    public List<CaseAssignee> CaseAssignees { get; set; } = new();
}