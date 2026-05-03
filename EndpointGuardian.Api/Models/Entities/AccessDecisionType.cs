namespace EndpointGuardian.Api.Models.Entities;

public enum AccessDecisionType
{
    Allow,
    Block,
    RequireMfa,
    RequireRemediation,
    RequireFreshCheckIn,
    Unknown
}