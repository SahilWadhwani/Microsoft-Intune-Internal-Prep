using EndpointGuardian.Api.Models;

namespace EndpointGuardian.Api.Services;

public interface IDeviceService
{
    List<ManagedDevice> GetAllDevices();
    ManagedDevice? GetDeviceById(string id);
    ManagedDevice? RegisterDevice(CreateDeviceRequest request);
    ManagedDevice? CheckInDevice(string id, CheckInDeviceRequest request);
    ComplianceEvaluationResult? EvaluateDevice(string id);
}