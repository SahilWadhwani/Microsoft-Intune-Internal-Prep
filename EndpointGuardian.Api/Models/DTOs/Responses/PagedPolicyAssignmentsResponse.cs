namespace EndpointGuardian.Api.Models.DTOs.Responses;

public record PagedPolicyAssignmentsResponse(

    List<PolicyAssignmentResponse> Items,

    int Page,

    int PageSize,

    int TotalCount

);