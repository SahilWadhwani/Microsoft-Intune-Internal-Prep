namespace EndpointGuardian.Api.Models.DTOs.Requests;

public record CreatePolicyRequest(
    string Name,
    int MinimumOsVersion,
    bool RequireEncryption,
    bool RequirePassword,
    bool RequireDefender,
    int MaxCheckInAgeHours
);