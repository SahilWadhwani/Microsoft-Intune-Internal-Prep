using EndpointGuardian.Api.Models;
using EndpointGuardian.Api.Repositories;

namespace EndpointGuardian.Api.Services;

public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly IComplianceEvaluator _complianceEvaluator;

    private readonly CompliancePolicyOptions _policyOptions;

    public DeviceService(
        IDeviceRepository deviceRepository,
        IComplianceEvaluator complianceEvaluator,
        IOptions<CompliancePolicyOptions> policyOptions)
    {
        _deviceRepository = deviceRepository;
        _complianceEvaluator = complianceEvaluator;
        _policyOptions = policyOptions.Value;
    }

    public List<ManagedDevice> GetAllDevices()
    {
        return _deviceRepository.GetAll();
    }

    public ManagedDevice? GetDeviceById(string id)
    {
        return _deviceRepository.GetById(id);
    }

    public ManagedDevice? RegisterDevice(CreateDeviceRequest request)
    {
        var existing = _deviceRepository.GetById(request.Id);
        if (existing is not null)
        {
            return null;
        }

        var device = new ManagedDevice
        {
            Id = request.Id,
            DeviceName = request.DeviceName,
            Platform = request.Platform,
            OsVersion = request.OsVersion,
            IsEncrypted = request.IsEncrypted,
            HasPassword = request.HasPassword,
            DefenderEnabled = request.DefenderEnabled,
            LastCheckInUtc = DateTime.UtcNow
        };

        _deviceRepository.Add(device);


        return device;
    }

    public ManagedDevice? CheckInDevice(string id, CheckInDeviceRequest request)
    {
        var device = _deviceRepository.GetById(id);
        if (device is null)
        {
            return null;
        }

        device.IsEncrypted = request.IsEncrypted;
        device.HasPassword = request.HasPassword;
        device.DefenderEnabled = request.DefenderEnabled;
        device.LastCheckInUtc = DateTime.UtcNow;

        _deviceRepository.Update(device);

        return device;
    }

    public ComplianceEvaluationResult? EvaluateDevice(string id)
    {
        var device = _deviceRepository.GetById(id);
        if (device is null)
        {
            return null;
        }

        var baselinePolicy = new CompliancePolicy
        {
            Id = "policy-001",
            Name = "Corporate Baseline",
            MinimumOsVersion = 13,
            RequireEncryption = true,
            RequirePassword = true,
            RequireDefender = true,
            MaxCheckInAgeHours = 72
        };

        return _complianceEvaluator.Evaluate(device, baselinePolicy);
    }
}