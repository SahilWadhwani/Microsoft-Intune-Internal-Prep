namespace EndpointGuardian.Api.Models.DTOs.Requests;

public record CreateAccessDecisionRequest(

    string UserId,
    
    string DeviceId,

    string Resource,

    AccessRiskLevel RiskLevel,

    bool MfaSatisfied

);