namespace EndpointGuardian.Api.ModelsDTOs.Requests;

public record CreateRemediationActionRequest(

    RemediationActionType ActionType,

    string? Reason

);