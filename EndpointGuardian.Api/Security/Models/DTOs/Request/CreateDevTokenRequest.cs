namespace EndpointGuardian.Api.Security;

public record CreateDevTokenRequest(
    string Subject,
    List<string> Permissions);