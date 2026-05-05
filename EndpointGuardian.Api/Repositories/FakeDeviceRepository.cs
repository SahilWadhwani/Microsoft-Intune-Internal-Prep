// InMemory implementation of IDeviceRepository for testing purposes
public class FakeDeviceRepository : IDeviceRepository
{
    private readonly List<ManagedDevice> _devices;

    public FakeDeviceRepository(List<ManagedDevice> devices)
    {
        _devices = devices;
    }

    public Task<List<ManagedDevice>> GetAllAsync()
    {
        return Task.FromResult(_devices);
    }

    public Task<ManagedDevice?> GetByIdAsync(string id)
    {
        return Task.FromResult(_devices.FirstOrDefault(d => d.Id == id));
    }

    public Task AddAsync(ManagedDevice device)
    {
        _devices.Add(device);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(ManagedDevice device)
    {
        return Task.CompletedTask;
    }
}