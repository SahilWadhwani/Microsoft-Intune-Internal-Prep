using EndpointGuardian.Api.Models;

namespace EndpointGuardian.Api.Services;

public interface IDeviceComplianceService
{
    DeviceComplianceEvaluationResult? EvaluateDevice(string deviceId);
}