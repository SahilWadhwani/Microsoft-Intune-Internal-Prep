using EndpointGuardian.Api.Models;

namespace EndpointGuardian.Api.Services;

public class BasicComplianceEvaluator : IComplianceEvaluator
{
    private readonly ILogger<BasicComplianceEvaluator> _logger;

    public BasicComplianceEvaluator(ILogger<BasicComplianceEvaluator> logger)
    {
        _logger = logger;
    }

    public PolicyEvaluationResult EvaluatePolicy(
        ManagedDevice device,
        CompliancePolicy policy)
    {
        if (device is null)
        {
            throw new ArgumentNullException(nameof(device));
        }

        if (policy is null)
        {
            throw new ArgumentNullException(nameof(policy));
        }

        _logger.LogInformation(
            "Evaluating device {DeviceId} against policy {PolicyId} version {PolicyVersion}",
            device.Id,
            policy.Id,
            policy.Version);

        var failures = new List<FailureReason>();

        EvaluateOsVersion(device, policy, failures);
        EvaluateEncryption(device, policy, failures);
        EvaluatePassword(device, policy, failures);
        EvaluateDefender(device, policy, failures);
        EvaluateCheckInFreshness(device, policy, failures);

        var status = failures.Count == 0
            ? ComplianceStatus.Compliant
            : ComplianceStatus.NonCompliant;

        _logger.LogInformation(
            "Device {DeviceId} evaluated against policy {PolicyId} with status {Status} and {FailureCount} failures",
            device.Id,
            policy.Id,
            status,
            failures.Count);

        return new PolicyEvaluationResult(
            policy.Id,
            policy.Name,
            policy.Version,
            status,
            failures
        );
    }

    private static void EvaluateOsVersion(
        ManagedDevice device,
        CompliancePolicy policy,
        List<FailureReason> failures)
    {
        if (device.OsVersion < policy.MinimumOsVersion)
        {
            failures.Add(new FailureReason(
                "OS_VERSION_TOO_LOW",
                $"Device OS version {device.OsVersion} is below required version {policy.MinimumOsVersion}."
            ));
        }
    }

    private static void EvaluateEncryption(
        ManagedDevice device,
        CompliancePolicy policy,
        List<FailureReason> failures)
    {
        if (!policy.RequireEncryption)
        {
            return;
        }

        if (device.CurrentPostureSnapshot?.IsEncrypted is null)
        {
            failures.Add(new FailureReason(
                "ENCRYPTION_NOT_REPORTED",
                "Device encryption is required but encryption status was not reported."
            ));
            return;
        }

        if (device.CurrentPostureSnapshot.IsEncrypted == false)
        {
            failures.Add(new FailureReason(
                "ENCRYPTION_DISABLED",
                "Device encryption is required but is reported as disabled."
            ));
        }
    }

    private static void EvaluatePassword(
        ManagedDevice device,
        CompliancePolicy policy,
        List<FailureReason> failures)
    {
        if (!policy.RequirePassword)
        {
            return;
        }

        if (device.CurrentPostureSnapshot?.HasPassword is null)
        {
            failures.Add(new FailureReason(
                "PASSWORD_NOT_REPORTED",
                "Device password requirement is enabled but password status was not reported."
            ));
            return;
        }

        if (device.CurrentPostureSnapshot.HasPassword == false)
        {
            failures.Add(new FailureReason(
                "PASSWORD_MISSING",
                "Device password requirement is enabled but device does not satisfy it."
            ));
        }
    }

    private static void EvaluateDefender(
        ManagedDevice device,
        CompliancePolicy policy,
        List<FailureReason> failures)
    {
        if (!policy.RequireDefender)
        {
            return;
        }

        if (device.CurrentPostureSnapshot?.DefenderEnabled is null)
        {
            failures.Add(new FailureReason(
                "DEFENDER_NOT_REPORTED",
                "Defender is required but Defender status was not reported."
            ));
            return;
        }

        if (device.CurrentPostureSnapshot.DefenderEnabled == false)
        {
            failures.Add(new FailureReason(
                "DEFENDER_DISABLED",
                "Defender is required but is reported as disabled."
            ));
        }
    }

    private static void EvaluateCheckInFreshness(
        ManagedDevice device,
        CompliancePolicy policy,
        List<FailureReason> failures)
    {
        var hoursSinceCheckIn = (DateTime.UtcNow - device.LastCheckInUtc).TotalHours;

        if (hoursSinceCheckIn > policy.MaxCheckInAgeHours)
        {
            failures.Add(new FailureReason(
                "CHECKIN_STALE",
                $"Device last checked in {Math.Round(hoursSinceCheckIn, 1)} hours ago, exceeding allowed limit of {policy.MaxCheckInAgeHours} hours."
            ));
        }
    }
}