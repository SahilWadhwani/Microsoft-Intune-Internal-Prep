public record DeviceResponse(
    string Id,
    string DeviceName,
    DevicePlatform Platform,
    int OsVersion,
    bool? IsEncrypted,
    bool? HasPassword,
    bool? DefenderEnabled,
    DateTime LastCheckInUtc,
    ComplianceStatus? CurrentComplianceStatus
);