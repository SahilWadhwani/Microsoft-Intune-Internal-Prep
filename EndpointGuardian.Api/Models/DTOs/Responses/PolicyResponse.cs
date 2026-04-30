namespace EndpointGuardian.Api.Models.DTOs.Responses;

public record PolicyResponse(

    string Id,

    string Name,

    int Version,

    int MinimumOsVersion,

    bool RequireEncryption,

    bool RequirePassword,

    bool RequireDefender,

    int MaxCheckInAgeHours,

    bool IsActive,

    DateTime CreatedAtUtc

);