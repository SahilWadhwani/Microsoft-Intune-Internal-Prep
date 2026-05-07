namespace EndpointGuardian.Api.Options;

public class ScheduledComplianceOptions

{

    public bool Enabled { get; set; } = true;

    public int IntervalSeconds { get; set; } = 300;

    public int MaxDevicesPerRun { get; set; } = 50;
    public int ReevaluateAfterMinutes { get; set; } = 60;
    public bool EvaluateOnlyStaleOrUnknownDevices { get; set; } = true;


}