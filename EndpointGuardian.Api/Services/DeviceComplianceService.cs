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

        _logger.LogInformation(
            "Starting compliance evaluation for device {DeviceId}",
            deviceId);

        var policies = _policyResolver.ResolvePoliciesForDevice(device);

        if (policies.Count == 0)
        {
            _logger.LogWarning(
                "Device {DeviceId} has no active assigned policies",
                deviceId);

            var noPolicyResult = new PolicyEvaluationResult(
                "none",
                "No active policy assigned",
                0,
                ComplianceStatus.Unknown,
                new List<FailureReason>
                {
                    new FailureReason(
                        "NO_ACTIVE_POLICY_ASSIGNED",
                        "No active compliance policy is assigned to this device."
                    )
                }
            );

            var unknownResult = new DeviceComplianceEvaluationResult(
                device.Id,
                ComplianceStatus.Unknown,
                new List<PolicyEvaluationResult> { noPolicyResult },
                DateTime.UtcNow
            );

            device.UpdateComplianceStatus(ComplianceStatus.Unknown);
            _deviceRepository.Update(device);

            return unknownResult;
        }

        var policyResults = policies
            .Select(policy => _complianceEvaluator.EvaluatePolicy(device, policy))
            .ToList();

        var overallStatus = policyResults.All(r => r.Status == ComplianceStatus.Compliant)
            ? ComplianceStatus.Compliant
            : ComplianceStatus.NonCompliant;

        var result = new DeviceComplianceEvaluationResult(
            device.Id,
            overallStatus,
            policyResults,
            DateTime.UtcNow
        );

        device.UpdateComplianceStatus(overallStatus);
        _deviceRepository.Update(device);

        _logger.LogInformation(
            "Completed compliance evaluation for device {DeviceId} with overall status {OverallStatus} across {PolicyCount} policies",
            device.Id,
            overallStatus,
            policies.Count);

        return result;
    }
}