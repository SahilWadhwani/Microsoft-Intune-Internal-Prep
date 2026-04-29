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

    public DeviceResponse? CreateDevice(CreateDeviceRequest request)
    {   
        if (string.IsNullOrWhiteSpace(request.DeviceName) || request.OsVersion <= 0)
        {
            return null;
        }

        var device = new ManagedDevice(
            Guid.NewGuid().ToString(),
            request.DeviceName,
            request.Platform);

        device.UpdateOsVersion(request.OsVersion);
        device.CheckIn(
            request.IsEncrypted,
            request.HasPassword,
            request.DefenderEnabled);

        _deviceRepository.Add(device);

        _logger.LogInformation(
            "Device {DeviceId} onboarded successfully on platform {Platform}",
            device.Id,
            device.Platform);

        return ToDeviceResponse(device);
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

    public PagedDevicesResponse GetDevices(GetDevicesQuery query)
    {

        var devices = _deviceRepository.GetAll().AsEnumerable();

        if (query.Platform is not null)
        {
            devices = devices.Where(d => d.Platform == query.Platform);
        }

        if (query.Status is not null)
        {
            devices = devices.Where(d => d.CurrentComplianceStatus == query.Status);
        }

        var totalCount = devices.Count();

        var items = devices
            .OrderBy(d => d.DeviceName)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(ToSummaryResponse)
            .ToList();

        return new PagedDevicesResponse(items, query.Page, query.PageSize, totalCount);
    }

    private static DeviceResponse ToDeviceResponse(ManagedDevice device)
    {
        return new DeviceResponse(
            device.Id,
            device.DeviceName,
            device.Platform,
            device.OsVersion,
            device.CurrentPostureSnapshot?.IsEncrypted,
            device.CurrentPostureSnapshot?.HasPassword,
            device.CurrentPostureSnapshot?.DefenderEnabled,
            device.LastCheckInUtc,
            device.CurrentComplianceStatus
        );
    }


    private static DeviceSummaryResponse ToSummaryResponse(ManagedDevice device)
    {
        return new DeviceSummaryResponse(
            device.Id,
            device.DeviceName,
            device.Platform,
            device.LastCheckInUtc,
            device.CurrentComplianceStatus
        );
    }



}