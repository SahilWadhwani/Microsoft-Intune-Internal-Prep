namespace EndpointGuardian.Api.Models.DTOs.Responses;

public record UpdateRemediationActionStatusRequest(

    RemediationStatus NewStatus,

    string ResultMessage

);