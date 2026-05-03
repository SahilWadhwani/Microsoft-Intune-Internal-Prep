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

        if (!policy.IsActive)
        {
            return new PolicyEvaluationResult(
                policy.Id,
                policy.Name,
                policy.Version,
                ComplianceStatus.Unknown,
                new List<FailureReason>
                {
                    new FailureReason(
                        "POLICY_INACTIVE",
                        "Policy is inactive and should not be used for compliance evaluation.",
                        FailureSeverity.Warning)
                },
                DateTime.UtcNow);
        }

        if (device.CurrentPostureSnapshot is null)
        {
            return new PolicyEvaluationResult(
                policy.Id,
                policy.Name,
                policy.Version,
                ComplianceStatus.Unknown,
                new List<FailureReason>
                {
                    new FailureReason(
                        "POSTURE_NOT_REPORTED",
                        "Device has not reported a posture snapshot, so compliance cannot be determined.",
                        FailureSeverity.High)
                },
                DateTime.UtcNow);
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

        return new PolicyEvaluationResult(
            policy.Id,
            policy.Name,
            policy.Version,
            status,
            failures,
            DateTime.UtcNow);
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
                $"Device OS version {device.OsVersion} is below required version {policy.MinimumOsVersion}.",
                FailureSeverity.High));
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

        var posture = device.CurrentPostureSnapshot!;

        if (posture.IsEncrypted is null)
        {
            failures.Add(new FailureReason(
                "ENCRYPTION_NOT_REPORTED",
                "Device encryption is required but encryption status was not reported.",
                FailureSeverity.High));
            return;
        }

        if (posture.IsEncrypted == false)
        {
            failures.Add(new FailureReason(
                "ENCRYPTION_DISABLED",
                "Device encryption is required but is reported as disabled.",
                FailureSeverity.High));
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

        var posture = device.CurrentPostureSnapshot!;

        if (posture.HasPassword is null)
        {
            failures.Add(new FailureReason(
                "PASSWORD_NOT_REPORTED",
                "Device password requirement is enabled but password status was not reported.",
                FailureSeverity.High));
            return;
        }

        if (posture.HasPassword == false)
        {
            failures.Add(new FailureReason(
                "PASSWORD_MISSING",
                "Device password requirement is enabled but device does not satisfy it.",
                FailureSeverity.High));
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

        var posture = device.CurrentPostureSnapshot!;

        if (posture.DefenderEnabled is null)
        {
            failures.Add(new FailureReason(
                "DEFENDER_NOT_REPORTED",
                "Defender is required but Defender status was not reported.",
                FailureSeverity.High));
            return;
        }

        if (posture.DefenderEnabled == false)
        {
            failures.Add(new FailureReason(
                "DEFENDER_DISABLED",
                "Defender is required but is reported as disabled.",
                FailureSeverity.High));
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
                $"Device last checked in {Math.Round(hoursSinceCheckIn, 1)} hours ago, exceeding allowed limit of {policy.MaxCheckInAgeHours} hours.",
                FailureSeverity.High));
        }
    }
}