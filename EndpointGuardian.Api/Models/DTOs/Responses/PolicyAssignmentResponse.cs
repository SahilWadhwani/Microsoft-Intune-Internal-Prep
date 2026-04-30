namespace Endpointguardian.Models.DTOs.Responses;

public record PolicyAssignmentResponse(

    string Id,

    string PolicyId,

    AssignmentTargetType TargetType,

    string? TargetId,

    DateTime AssignedAtUtc,

    string AssignedBy,

    bool IsActive

);