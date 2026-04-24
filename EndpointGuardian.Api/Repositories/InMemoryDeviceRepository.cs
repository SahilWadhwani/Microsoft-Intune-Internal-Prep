using EndpointGuardian.Api.Models;
using EndpointGuardian.Api.Repositories;

namespace EndpointGuardian.Api.Repositories;

public class InMemoryDeviceRepository : IDeviceRepository
{
    private readonly List<ManagedDevice> _devices = new();

    public List<ManagedDevice> GetAll()
    {
        return _devices;
    }

    public ManagedDevice? GetById(string id)
    {
        return _devices.FirstOrDefault(d => d.Id == id);
    }

    public void Add(ManagedDevice device)
    {
        _devices.Add(device);
    }

    public void Update(ManagedDevice device)
    {
        // In-memory list already references the same object.
        // Kept here for abstraction symmetry and future persistence changes.
    }
}