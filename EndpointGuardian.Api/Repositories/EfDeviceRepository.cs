using EndpointGuardian.Api.Data;
using EndpointGuardian.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EndpointGuardian.Api.Repositories;

public class EfDeviceRepository : IDeviceRepository
{
    private readonly EndpointGuardianDbContext _dbContext;

    public EfDeviceRepository(EndpointGuardianDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ManagedDevice>> GetAllAsync()
    {
        return await _dbContext.Devices.ToListAsync();
    }

    public async Task<ManagedDevice?> GetByIdAsync(string id)
    {
        return await _dbContext.Devices
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task AddAsync(ManagedDevice device)
    {
        await _dbContext.Devices.AddAsync(device);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(ManagedDevice device)
    {
        _dbContext.Devices.Update(device);
        await _dbContext.SaveChangesAsync();
    }
}