namespace EndpointGuardian.Api.Models.DTOs.Requests;

public record GetAuditEventsQuery(

    AuditActionType? ActionType,

    AuditEntityType? EntityType,

    string? EntityId,

    string? Actor,

    int Page = 1,

    int PageSize = 20

);