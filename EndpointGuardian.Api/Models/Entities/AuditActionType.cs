namespace EndpointGuardian.Api.Models.DTOs.Responses;

public enum AuditActionType

{

    DeviceCreated,

    PolicyCreated,

    PolicyAssigned,

    ComplianceEvaluated,

    AccessDecisionCreated,

    RemediationRequested,

    RemediationStatusChanged

}