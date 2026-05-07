namespace EndpointGuardian.Api.Models.DTOs.Responses;

public record AuditEventResponse(

    string Id,

    AuditActionType ActionType,

    AuditEntityType EntityType,

    string EntityId,

    string Actor,

    DateTime TimestampUtc,

    string Summary

);