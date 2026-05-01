namespace EndpointGuardian.Api.Models.DTOs.Responses;

public record PolicyEvaluationResult(

    string PolicyId,

    string PolicyName,

    int PolicyVersion,

    ComplianceStatus Status,

    List<FailureReason> FailureReasons

);