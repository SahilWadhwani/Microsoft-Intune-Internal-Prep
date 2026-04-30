namespace EndpointGuardian.Api.Models;

public enum AssignmentTargetType
{
    Device,
    Platform,
    AllDevices
}

public class PolicyAssignment
{
    public string Id { get; private set; }
    public string PolicyId { get; private set; }
    public AssignmentTargetType TargetType { get; private set; }
    public string? TargetId { get; private set; }
    public DateTime AssignedAtUtc { get; private set; }
    public string AssignedBy { get; private set; }
    public bool IsActive { get; private set; }

    public PolicyAssignment(
        string id,
        string policyId,
        AssignmentTargetType targetType,
        string? targetId,
        string assignedBy)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Assignment id cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(policyId))
        {
            throw new ArgumentException("Policy id cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(assignedBy))
        {
            throw new ArgumentException("Assigned by cannot be empty.");
        }

        if (targetType != AssignmentTargetType.AllDevices &&
            string.IsNullOrWhiteSpace(targetId))
        {
            throw new ArgumentException("Target id is required for this assignment target type.");
        }

        Id = id;
        PolicyId = policyId;
        TargetType = targetType;
        TargetId = targetId;
        AssignedBy = assignedBy;
        AssignedAtUtc = DateTime.UtcNow;
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}