namespace EndpointGuardian.Api.Models.DTOs.Responses;

public record GetPolicyAssignmentsQuery(

    bool? IsActive,

    int Page = 1,

    int PageSize = 20

);