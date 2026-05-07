namespace EndpointGuardian.Api.Models.DTOs.Responses;

public record RemediationActionResponse(
    string Id,
    string DeviceId,
    RemediationActionType ActionType,
    RemediationStatus Status,
    string RequestedBy,
    DateTime RequestedAtUtc,
    DateTime? StartedAtUtc,
    DateTime? CompletedAtUtc,
    string? ResultMessage
);