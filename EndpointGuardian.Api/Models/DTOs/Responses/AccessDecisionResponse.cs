namespace EndpointGuardian.Api.Models.DTOs.Responses;

public record AccessDecisionResponse(

    string DecisionId,

    string UserId,

    string DeviceId,

    string Resource,

    AccessDecisionType Decision,

    List<AccessDecisionReason> Reasons,

    DateTime DecidedAtUtc

);