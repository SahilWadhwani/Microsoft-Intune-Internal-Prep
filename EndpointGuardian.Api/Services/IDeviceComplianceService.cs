using EndpointGuardian.Api.Models;

namespace EndpointGuardian.Api.Services;

public interface IDeviceComplianceService
{
    Task<DeviceComplianceEvaluationResult?> EvaluateDeviceAsync(string deviceId);
}