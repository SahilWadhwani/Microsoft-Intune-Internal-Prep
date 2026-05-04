using EndpointGuardian.Api.Models;

namespace EndpointGuardian.Api.Repositories;


public interface IDeviceRepository

{

    Task<List<ManagedDevice>> GetAllAsync();

    Task<ManagedDevice?> GetByIdAsync(string id);

    Task AddAsync(ManagedDevice device);

    Task UpdateAsync(ManagedDevice device);

}