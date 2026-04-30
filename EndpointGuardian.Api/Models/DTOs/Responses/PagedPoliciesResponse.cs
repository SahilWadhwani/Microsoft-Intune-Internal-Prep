namespace EndpointGuardian.Api.Models.DTOs.Responses;

public record PagedPoliciesResponse(
    List<PolicySummaryResponse> Items,
    int Page,
    int PageSize,
    int TotalCount
);