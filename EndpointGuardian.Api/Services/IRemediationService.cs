using EndpointGuardian.Api.Models;

namespace EndpointGuardian.Api.Services;

public interface IRemediationService
{
    Task<RemediationActionResponse?> CreateRemediationActionAsync(
        string deviceId,
        CreateRemediationActionRequest request,
        string actor);

    Task<PagedRemediationActionsResponse?> GetRemediationActionsForDeviceAsync(
        string deviceId,
        GetRemediationActionsQuery query);

    Task<RemediationActionResponse?> GetByIdAsync(string actionId);

    Task<RemediationActionResponse?> UpdateStatusAsync(
        string actionId,
        UpdateRemediationActionStatusRequest request,
        string actor);
}