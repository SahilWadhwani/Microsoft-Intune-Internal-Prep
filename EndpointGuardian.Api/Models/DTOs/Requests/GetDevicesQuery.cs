public record GetDevicesQuery(
    DevicePlatform? Platform,
    ComplianceStatus? ComplianceStatus,
    int page = 1,
    int PageSize = 20
);