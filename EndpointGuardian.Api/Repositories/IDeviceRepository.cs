using EndpointGuardian.Api.Models;

namespace EndpointGuardian.Api.Repositories;


public interface IDeviceRepository
{
    List<ManagedDevice> GetAll();
    ManagedDevice? GetById(string id);
    void Add(ManagedDevice device);
    void Update(ManagedDevice device);
}