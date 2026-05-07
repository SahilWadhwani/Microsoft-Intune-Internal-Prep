using EndpointGuardian.Api.Models;
using EndpointGuardian.Api.Repositories;

namespace EndpointGuardian.Api.Services;

public class RemediationService : IRemediationService
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly IRemediationActionRepository _remediationRepository;
    private readonly IAuditService _auditService;
    private readonly ILogger<RemediationService> _logger;

    public RemediationService(
        IDeviceRepository deviceRepository,
        IRemediationActionRepository remediationRepository,
        IAuditService auditService,
        ILogger<RemediationService> logger)
    {
        _deviceRepository = deviceRepository;
        _remediationRepository = remediationRepository;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<RemediationActionResponse?> CreateRemediationActionAsync(
        string deviceId,
        CreateRemediationActionRequest request,
        string actor)
    {
        var device = await _deviceRepository.GetByIdAsync(deviceId);

        if (device is null)
        {
            _logger.LogWarning(
                "Cannot create remediation action because device {DeviceId} was not found",
                deviceId);

            return null;
        }

        if (device.CurrentComplianceStatus == ComplianceStatus.Compliant &&
            request.ActionType != RemediationActionType.ManualReview)
        {
            _logger.LogWarning(
                "Remediation action {ActionType} rejected because device {DeviceId} is compliant",
                request.ActionType,
                deviceId);

            return null;
        }

        var existingActions = await _remediationRepository.GetByDeviceIdAsync(deviceId);

        var duplicateActiveAction = existingActions.Any(a =>
            a.ActionType == request.ActionType &&
            (a.Status == RemediationStatus.Pending ||
             a.Status == RemediationStatus.InProgress));

        if (duplicateActiveAction)
        {
            _logger.LogWarning(
                "Duplicate active remediation action {ActionType} rejected for device {DeviceId}",
                request.ActionType,
                deviceId);

            return null;
        }

        var action = new RemediationAction(
            Guid.NewGuid().ToString(),
            deviceId,
            request.ActionType,
            actor);

        await _remediationRepository.AddAsync(action);

        await _auditService.RecordAsync(
            AuditActionType.RemediationRequested,
            AuditEntityType.RemediationAction,
            action.Id,
            actor,
            $"Requested remediation action {action.ActionType} for device {deviceId}.");

        _logger.LogInformation(
            "Remediation action {RemediationActionId} requested for device {DeviceId} by {Actor}",
            action.Id,
            deviceId,
            actor);

        return ToResponse(action);
    }

    public async Task<PagedRemediationActionsResponse?> GetRemediationActionsForDeviceAsync(
        string deviceId,
        GetRemediationActionsQuery query)
    {
        var device = await _deviceRepository.GetByIdAsync(deviceId);

        if (device is null)
            return null;

        var actions = (await _remediationRepository.GetByDeviceIdAsync(deviceId))
            .AsEnumerable();

        if (query.Status is not null)
            actions = actions.Where(a => a.Status == query.Status);

        var totalCount = actions.Count();

        var items = actions
            .OrderByDescending(a => a.RequestedAtUtc)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(ToResponse)
            .ToList();

        return new PagedRemediationActionsResponse(
            items,
            query.Page,
            query.PageSize,
            totalCount);
    }

    public async Task<RemediationActionResponse?> GetByIdAsync(string actionId)
    {
        var action = await _remediationRepository.GetByIdAsync(actionId);
        return action is null ? null : ToResponse(action);
    }

    public async Task<RemediationActionResponse?> UpdateStatusAsync(
        string actionId,
        UpdateRemediationActionStatusRequest request,
        string actor)
    {
        var action = await _remediationRepository.GetByIdAsync(actionId);

        if (action is null)
            return null;

        switch (request.NewStatus)
        {
            case RemediationStatus.InProgress:
                action.Start();
                break;

            case RemediationStatus.Completed:
                action.Complete(request.ResultMessage);
                break;

            case RemediationStatus.Failed:
                action.Fail(request.ResultMessage);
                break;

            case RemediationStatus.Cancelled:
                action.Cancel(request.ResultMessage);
                break;

            default:
                throw new InvalidOperationException("Unsupported remediation status transition.");
        }

        await _remediationRepository.UpdateAsync(action);

        await _auditService.RecordAsync(
            AuditActionType.RemediationStatusChanged,
            AuditEntityType.RemediationAction,
            action.Id,
            actor,
            $"Changed remediation action status to {action.Status}. Result: {request.ResultMessage}");

        return ToResponse(action);
    }

    private static RemediationActionResponse ToResponse(RemediationAction action)
    {
        return new RemediationActionResponse(
            action.Id,
            action.DeviceId,
            action.ActionType,
            action.Status,
            action.RequestedBy,
            action.RequestedAtUtc,
            action.StartedAtUtc,
            action.CompletedAtUtc,
            action.ResultMessage);
    }
}