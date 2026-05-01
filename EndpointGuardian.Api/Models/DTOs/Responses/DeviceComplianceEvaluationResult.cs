namespace EndpointGuardian.Api.Models.DTOs.Responses;
public record DeviceComplianceEvaluationResult(

    string DeviceId,

    ComplianceStatus OverallStatus,

    List<PolicyEvaluationResult> PolicyResults,

    DateTime EvaluatedAtUtc

);