namespace EndpointGuardian.Api.Models.Entities;

public enum AuditEntityType
{
    Device,

    Policy,

    PolicyAssignment,

    ComplianceEvaluation,

    AccessDecision,

    RemediationAction
}