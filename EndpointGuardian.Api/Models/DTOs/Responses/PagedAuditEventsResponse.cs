namespace EndpointGuardian.Api.Models.DTOs.Responses;

public record PagedAuditEventsResponse(

    List<AuditEventResponse> Items,

    int Page,

    int PageSize,

    int TotalCount

);