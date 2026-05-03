namespace EndpointGuardian.Api.Models.Entities;

public enum FailureSeverity
{
    Info,
    Warning,
    Critical,
    High
    
}
public record FailureReason(

    string Code,

    string Message,

    FailureSeverity Severity

);