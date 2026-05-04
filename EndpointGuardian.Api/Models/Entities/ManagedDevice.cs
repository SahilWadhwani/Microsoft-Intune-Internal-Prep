namespace EndpointGuardian.Api.Models.Entities;

public class ManagedDevice
{
    private ManagedDevice()
    {
        Id = "";
        DeviceName = "";
    }
    public string Id { get; private set; }
    public string DeviceName { get; private set; }
    public DevicePlatform Platform { get; private set; }
    public int OsVersion { get; private set; }
    public DateTime LastCheckInUtc { get; private set; }
    public ComplianceStatus? CurrentComplianceStatus { get; private set; }
    public DevicePostureSnapshot? CurrentPostureSnapshot { get; private set; }

    public ManagedDevice(
        string id,
        string deviceName,
        DevicePlatform platform)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Device id cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(deviceName))
        {
            throw new ArgumentException("Device name cannot be empty.");
        }

        Id = id;
        DeviceName = deviceName;
        Platform = platform;
        LastCheckInUtc = DateTime.UtcNow;
    }

    public void UpdateOsVersion(int osVersion)
    {
        if (osVersion <= 0)
        {
            throw new ArgumentException("OS version must be greater than zero.");
        }

        OsVersion = osVersion;
    }

    public void CheckIn(
        bool? isEncrypted,
        bool? hasPassword,
        bool? defenderEnabled)
    {
        CurrentPostureSnapshot = new DevicePostureSnapshot(
            isEncrypted,
            hasPassword,
            defenderEnabled,
            DateTime.UtcNow);

        LastCheckInUtc = DateTime.UtcNow;
    }

    public void UpdateComplianceStatus(ComplianceStatus status)
    {
        CurrentComplianceStatus = status;
    }
}