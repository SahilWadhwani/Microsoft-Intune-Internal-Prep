namespace EndpointGuardian.Api.Models.DTOs.Responses;

public record GetPoliciesQuery(

    bool? IsActive,

    int Page = 1,

    int PageSize = 20

);