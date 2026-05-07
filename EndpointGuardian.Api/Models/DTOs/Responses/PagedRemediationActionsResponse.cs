namespace EndpointGuardian.Api.Models.DTOs.Responses;

public record PagedRemediationActionsResponse(

    List<RemediationActionResponse> Items,

    int Page,

    int PageSize,

    int TotalCount

);