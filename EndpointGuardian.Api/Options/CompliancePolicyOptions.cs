namespace EndpointGuardian.Api.Options;

public class CompliancePolicyOptions

{

    public int MinimumOsVersion { get; set; }

    public bool RequireEncryption { get; set; }

    public bool RequirePassword { get; set; }

    public bool RequireDefender { get; set; }

    public int MaxCheckInAgeHours { get; set; }

}