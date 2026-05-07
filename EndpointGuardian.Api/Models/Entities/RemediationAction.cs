namespace EndpointGuardian.Api.Models;

public class RemediationAction
{
    private RemediationAction()
    {
        Id = "";
        DeviceId = "";
        RequestedBy = "";
    }

    public string Id { get; private set; }
    public string DeviceId { get; private set; }
    public RemediationActionType ActionType { get; private set; }
    public RemediationStatus Status { get; private set; }
    public string RequestedBy { get; private set; }
    public DateTime RequestedAtUtc { get; private set; }
    public DateTime? StartedAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public string? ResultMessage { get; private set; }

    public RemediationAction(
        string id,
        string deviceId,
        RemediationActionType actionType,
        string requestedBy)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Remediation action id cannot be empty.");

        if (string.IsNullOrWhiteSpace(deviceId))
            throw new ArgumentException("Device id cannot be empty.");

        if (string.IsNullOrWhiteSpace(requestedBy))
            throw new ArgumentException("Requested by cannot be empty.");

        Id = id;
        DeviceId = deviceId;
        ActionType = actionType;
        RequestedBy = requestedBy;
        Status = RemediationStatus.Pending;
        RequestedAtUtc = DateTime.UtcNow;
    }

    public void Start()
    {
        if (Status != RemediationStatus.Pending)
            throw new InvalidOperationException("Only pending remediation actions can be started.");

        Status = RemediationStatus.InProgress;
        StartedAtUtc = DateTime.UtcNow;
    }

    public void Complete(string resultMessage)
    {
        if (Status != RemediationStatus.InProgress)
            throw new InvalidOperationException("Only in-progress remediation actions can be completed.");

        Status = RemediationStatus.Completed;
        CompletedAtUtc = DateTime.UtcNow;
        ResultMessage = resultMessage;
    }

    public void Fail(string resultMessage)
    {
        if (Status != RemediationStatus.InProgress)
            throw new InvalidOperationException("Only in-progress remediation actions can fail.");

        Status = RemediationStatus.Failed;
        CompletedAtUtc = DateTime.UtcNow;
        ResultMessage = resultMessage;
    }

    public void Cancel(string resultMessage)
    {
        if (Status != RemediationStatus.Pending &&
            Status != RemediationStatus.InProgress)
        {
            throw new InvalidOperationException("Only pending or in-progress remediation actions can be cancelled.");
        }

        Status = RemediationStatus.Cancelled;
        CompletedAtUtc = DateTime.UtcNow;
        ResultMessage = resultMessage;
    }
}