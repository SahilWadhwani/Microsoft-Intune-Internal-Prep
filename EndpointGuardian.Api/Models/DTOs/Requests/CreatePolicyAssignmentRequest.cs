namespace Endpointguardian.Models.DTOs.Requests;

public record CreatePolicyAssignmentRequest(

    AssignmentTargetType TargetType,

    string? TargetId,

    string AssignedBy

);