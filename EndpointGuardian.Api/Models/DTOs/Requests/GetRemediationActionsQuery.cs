namespace EndpointGuardian.Api.Models.DTOs.Responses;

public record GetRemediationActionsQuery(

    RemediationStatus? Status,

    int Page = 1,

    int PageSize = 20

);