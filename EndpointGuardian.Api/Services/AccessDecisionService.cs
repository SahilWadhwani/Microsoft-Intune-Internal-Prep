using EndpointGuardian.Api.Models;
using EndpointGuardian.Api.Repositories;

namespace EndpointGuardian.Api.Services;

public class AccessDecisionService : IAccessDecisionService
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly ILogger<AccessDecisionService> _logger;

    public AccessDecisionService(
        IDeviceRepository deviceRepository,
        ILogger<AccessDecisionService> logger)
    {
        _deviceRepository = deviceRepository;
        _logger = logger;
    }

    public AccessDecisionResponse? CreateDecision(CreateAccessDecisionRequest request)
    {
        var device = _deviceRepository.GetById(request.DeviceId);

        if (device is null)
        {
            _logger.LogWarning(
                "Access decision could not be created because device {DeviceId} was not found",
                request.DeviceId);

            return null;
        }

        var reasons = new List<AccessDecisionReason>();
        var decision = Decide(device, request, reasons);

        var response = new AccessDecisionResponse(
            Guid.NewGuid().ToString(),
            request.UserId,
            request.DeviceId,
            request.Resource,
            decision,
            reasons,
            DateTime.UtcNow
        );

        _logger.LogInformation(
            "Access decision {DecisionId} for user {UserId}, device {DeviceId}, resource {Resource}: {Decision}",
            response.DecisionId,
            response.UserId,
            response.DeviceId,
            response.Resource,
            response.Decision);

        return response;
    }

    private static AccessDecisionType Decide(
        ManagedDevice device,
        CreateAccessDecisionRequest request,
        List<AccessDecisionReason> reasons)
    {
        if (device.CurrentComplianceStatus is null ||
            device.CurrentComplianceStatus == ComplianceStatus.Unknown)
        {
            reasons.Add(new AccessDecisionReason(
                "DEVICE_COMPLIANCE_UNKNOWN",
                "Device compliance status is unknown. A fresh compliance evaluation or check-in is required."));

            return AccessDecisionType.RequireFreshCheckIn;
        }

        if (device.CurrentComplianceStatus == ComplianceStatus.Error)
        {
            reasons.Add(new AccessDecisionReason(
                "DEVICE_COMPLIANCE_ERROR",
                "Device compliance evaluation encountered an error, so access is blocked."));

            return AccessDecisionType.Block;
        }

        if (device.CurrentComplianceStatus == ComplianceStatus.NonCompliant)
        {
            reasons.Add(new AccessDecisionReason(
                "DEVICE_NONCOMPLIANT",
                "Device is noncompliant and must be remediated before access can be granted."));

            return AccessDecisionType.RequireRemediation;
        }

        if (request.RiskLevel == AccessRiskLevel.High)
        {
            reasons.Add(new AccessDecisionReason(
                "HIGH_RISK_SESSION",
                "Session risk is high, so access is blocked even though the device is compliant."));

            return AccessDecisionType.Block;
        }

        if (request.RiskLevel == AccessRiskLevel.Medium &&
            !request.MfaSatisfied)
        {
            reasons.Add(new AccessDecisionReason(
                "MFA_REQUIRED",
                "Medium-risk access requires multifactor authentication."));

            return AccessDecisionType.RequireMfa;
        }

        reasons.Add(new AccessDecisionReason(
            "ACCESS_REQUIREMENTS_SATISFIED",
            "Device is compliant and access requirements are satisfied."));

        return AccessDecisionType.Allow;
    }
}