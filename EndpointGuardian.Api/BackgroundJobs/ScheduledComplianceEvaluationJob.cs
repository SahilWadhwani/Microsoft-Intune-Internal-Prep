using EndpointGuardian.Api.Models;
using EndpointGuardian.Api.Options;
using EndpointGuardian.Api.Repositories;
using EndpointGuardian.Api.Services;
using Microsoft.Extensions.Options;

namespace EndpointGuardian.Api.BackgroundJobs;

public class ScheduledComplianceEvaluationJob : IScheduledComplianceEvaluationJob
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly IDeviceComplianceService _deviceComplianceService;
    private readonly IAuditService _auditService;
    private readonly ScheduledComplianceOptions _options;
    private readonly ILogger<ScheduledComplianceEvaluationJob> _logger;

    public ScheduledComplianceEvaluationJob(
        IDeviceRepository deviceRepository,
        IDeviceComplianceService deviceComplianceService,
        IAuditService auditService,
        IOptions<ScheduledComplianceOptions> options,
        ILogger<ScheduledComplianceEvaluationJob> logger)
    {
        _deviceRepository = deviceRepository;
        _deviceComplianceService = deviceComplianceService;
        _auditService = auditService;
        _options = options.Value;
        _logger = logger;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var allDevices = await _deviceRepository.GetAllAsync();

        var devicesToEvaluate = allDevices
            .Where(ShouldEvaluate)
            .Take(_options.MaxDevicesPerRun)
            .ToList();

        _logger.LogInformation(
            "Scheduled compliance job found {DeviceCount} devices to evaluate out of {TotalDeviceCount}.",
            devicesToEvaluate.Count,
            allDevices.Count);

        foreach (var device in devicesToEvaluate)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _deviceComplianceService.EvaluateDeviceAsync(device.Id);

            if (result is null)
            {
                _logger.LogWarning(
                    "Scheduled compliance job skipped device {DeviceId} because evaluation returned null.",
                    device.Id);

                continue;
            }

            _logger.LogInformation(
                "Scheduled compliance evaluation completed for device {DeviceId} with status {Status}.",
                device.Id,
                result.OverallStatus);

            if (result.OverallStatus == ComplianceStatus.NonCompliant ||
                result.OverallStatus == ComplianceStatus.Unknown ||
                result.OverallStatus == ComplianceStatus.Error)
            {
                await _auditService.RecordAsync(
                    AuditActionType.ComplianceEvaluated,
                    AuditEntityType.ComplianceEvaluation,
                    result.EvaluationId,
                    "system:scheduled-compliance-worker",
                    $"Scheduled compliance evaluation for device {device.Id} completed with status {result.OverallStatus}.");
            }
        }
    }

    private bool ShouldEvaluate(ManagedDevice device)
    {
        if (!_options.EvaluateOnlyStaleOrUnknownDevices)
        {
            return true;
        }

        if (device.CurrentComplianceStatus is null ||
            device.CurrentComplianceStatus == ComplianceStatus.Unknown)
        {
            return true;
        }

        if (device.LastComplianceEvaluationAtUtc is null)
        {
            return true;
        }

        var age = DateTime.UtcNow - device.LastComplianceEvaluationAtUtc.Value;

        return age.TotalMinutes >= _options.ReevaluateAfterMinutes;
    }
}