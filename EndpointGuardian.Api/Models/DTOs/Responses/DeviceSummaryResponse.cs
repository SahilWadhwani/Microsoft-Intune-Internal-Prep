public record DeviceSummaryResponse(
    string Id,
    string DeviceName,
    DevicePlatform Platform,
    DateTime LastCheckInUtc,
    ComplianceStatus? CurrentComplianceStatus
);