public record ComplianceEvaluationResponse(
    string DeviceId,
    ComplianceStatus Status,
    List<string> FailureReasons,
    DateTime EvaluatedAtUtc
);