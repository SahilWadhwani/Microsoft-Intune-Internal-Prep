using EndpointGuardian.Api.Models;

namespace EndpointGuardian.Api.Services;

public interface IAuditService

{

    Task RecordAsync(

        AuditActionType actionType,

        AuditEntityType entityType,

        string entityId,

        string actor,

        string summary);

    Task<PagedAuditEventsResponse> GetAuditEventsAsync(GetAuditEventsQuery query);

}