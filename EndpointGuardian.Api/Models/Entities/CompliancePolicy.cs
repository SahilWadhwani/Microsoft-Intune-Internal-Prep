namespace EndpointGuardian.Api.Models;

public class CompliancePolicy
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public int Version { get; private set; }
    public int MinimumOsVersion { get; private set; }
    public bool RequireEncryption { get; private set; }
    public bool RequirePassword { get; private set; }
    public bool RequireDefender { get; private set; }
    public int MaxCheckInAgeHours { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public CompliancePolicy(
        string id,
        string name,
        int minimumOsVersion,
        bool requireEncryption,
        bool requirePassword,
        bool requireDefender,
        int maxCheckInAgeHours)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Policy id cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Policy name cannot be empty.");
        }

        if (minimumOsVersion <= 0)
        {
            throw new ArgumentException("Minimum OS version must be greater than zero.");
        }

        if (maxCheckInAgeHours <= 0)
        {
            throw new ArgumentException("Max check-in age must be greater than zero.");
        }

        Id = id;
        Name = name;
        MinimumOsVersion = minimumOsVersion;
        RequireEncryption = requireEncryption;
        RequirePassword = requirePassword;
        RequireDefender = requireDefender;
        MaxCheckInAgeHours = maxCheckInAgeHours;
        Version = 1;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Reactivate()
    {
        IsActive = true;
    }
}