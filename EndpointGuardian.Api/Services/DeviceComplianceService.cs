using EndpointGuardian.Api.Models;
using EndpointGuardian.Api.Repositories;

namespace EndpointGuardian.Api.Services;

public class DeviceComplianceService : IDeviceComplianceService
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly IEffectivePolicyResolver _policyResolver;
    private readonly IComplianceEvaluator _complianceEvaluator;
    private readonly ILogger<DeviceComplianceService> _logger;

    public DeviceComplianceService(
        IDeviceRepository deviceRepository,
        IEffectivePolicyResolver policyResolver,
        IComplianceEvaluator complianceEvaluator,
        ILogger<DeviceComplianceService> logger)
    {
        _deviceRepository = deviceRepository;
        _policyResolver = policyResolver;
        _complianceEvaluator = complianceEvaluator;
        _logger = logger;
    }

    public DeviceComplianceEvaluationResult? EvaluateDevice(string deviceId)
    {
        var device = _deviceRepository.GetById(deviceId);

        if (device is null)
        {
            _logger.LogWarning(
                "Cannot evaluate compliance because device {DeviceId} was not found",
                deviceId);

            return null;
        }

        var policies = _policyResolver.ResolvePoliciesForDevice(device);

        if (policies.Count == 0)
        {
            var unknownResult = CreateNoPolicyResult(device);

            device.UpdateComplianceStatus(ComplianceStatus.Unknown);
            _deviceRepository.Update(device);

            return unknownResult;
        }

        var policyResults = new List<PolicyEvaluationResult>();

        foreach (var policy in policies)
        {
            try
            {
                var result = _complianceEvaluator.EvaluatePolicy(device, policy);
                policyResults.Add(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Evaluation failed for device {DeviceId} and policy {PolicyId}",
                    device.Id,
                    policy.Id);

                policyResults.Add(new PolicyEvaluationResult(
                    policy.Id,
                    policy.Name,
                    policy.Version,
                    ComplianceStatus.Error,
                    new List<FailureReason>
                    {
                        new FailureReason(
                            "EVALUATION_ERROR",
                            "An unexpected error occurred while evaluating this policy.",
                            FailureSeverity.Critical)
                    },
                    DateTime.UtcNow));
            }
        }

        var overallStatus = CombinePolicyResults(policyResults);

        var evaluation = new DeviceComplianceEvaluationResult(
            Guid.NewGuid().ToString(),
            device.Id,
            overallStatus,
            policyResults,
            DateTime.UtcNow);

        device.UpdateComplianceStatus(overallStatus);
        _deviceRepository.Update(device);

        _logger.LogInformation(
            "Compliance evaluation {EvaluationId} completed for device {DeviceId} with status {OverallStatus}",
            evaluation.EvaluationId,
            device.Id,
            overallStatus);

        return evaluation;
    }

    private static DeviceComplianceEvaluationResult CreateNoPolicyResult(ManagedDevice device)
    {
        var noPolicyResult = new PolicyEvaluationResult(
            "none",
            "No active policy assigned",
            0,
            ComplianceStatus.Unknown,
            new List<FailureReason>
            {
                new FailureReason(
                    "NO_ACTIVE_POLICY_ASSIGNED",
                    "No active compliance policy is assigned to this device.",
                    FailureSeverity.Warning)
            },
            DateTime.UtcNow);

        return new DeviceComplianceEvaluationResult(
            Guid.NewGuid().ToString(),
            device.Id,
            ComplianceStatus.Unknown,
            new List<PolicyEvaluationResult> { noPolicyResult },
            DateTime.UtcNow);
    }

    private static ComplianceStatus CombinePolicyResults(
        List<PolicyEvaluationResult> policyResults)
    {
        if (policyResults.Count == 0)
        {
            return ComplianceStatus.Unknown;
        }

        if (policyResults.Any(r => r.Status == ComplianceStatus.Error))
        {
            return ComplianceStatus.Error;
        }

        if (policyResults.Any(r => r.Status == ComplianceStatus.NonCompliant))
        {
            return ComplianceStatus.NonCompliant;
        }

        if (policyResults.Any(r => r.Status == ComplianceStatus.Unknown))
        {
            return ComplianceStatus.Unknown;
        }

        return ComplianceStatus.Compliant;
    }
}