public record PagedDevicesResponse(
    List<DeviceSummaryResponse> Items,
    int Page,
    int PageSize,
    int TotalCount
);