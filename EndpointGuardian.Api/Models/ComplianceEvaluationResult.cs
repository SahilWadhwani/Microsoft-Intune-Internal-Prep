public enum ComplianceStatus
{
    Compliant,
    NonCompliant,
    Unknown,
    Error
}


public record ComplianceEvaluationResult (
    string DeviceId,
    ComplianceStatus Status,
    List<string> FailureReasons,
    DateTime EvaluatedAtUtc
);


