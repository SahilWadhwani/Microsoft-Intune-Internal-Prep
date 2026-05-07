using EndpointGuardian.Api.Models;

namespace EndpointGuardian.Api.Repositories;

public interface IRemediationActionRepository
{
    Task<List<RemediationAction>> GetAllAsync();
    Task<List<RemediationAction>> GetByDeviceIdAsync(string deviceId);
    Task<RemediationAction?> GetByIdAsync(string id);
    Task AddAsync(RemediationAction action);
    Task UpdateAsync(RemediationAction action);
}