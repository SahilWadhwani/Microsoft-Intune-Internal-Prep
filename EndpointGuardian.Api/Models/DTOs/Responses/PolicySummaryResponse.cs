namespace EndpointGuardian.Api.Models.DTOs.Responses;

public record PolicySummaryResponse(
    string Id,
    string Name,
    int Version,
    bool IsActive,
    DateTime CreatedAtUtc
);