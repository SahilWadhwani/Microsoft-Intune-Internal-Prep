using EndpointGuardian.Api.Data;
using EndpointGuardian.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EndpointGuardian.Api.Repositories;

public class EfRemediationActionRepository : IRemediationActionRepository
{
    private readonly EndpointGuardianDbContext _dbContext;

    public EfRemediationActionRepository(EndpointGuardianDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<RemediationAction>> GetAllAsync()
    {
        return await _dbContext.RemediationActions.ToListAsync();
    }

    public async Task<List<RemediationAction>> GetByDeviceIdAsync(string deviceId)
    {
        return await _dbContext.RemediationActions
            .Where(a => a.DeviceId == deviceId)
            .ToListAsync();
    }

    public async Task<RemediationAction?> GetByIdAsync(string id)
    {
        return await _dbContext.RemediationActions
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task AddAsync(RemediationAction action)
    {
        await _dbContext.RemediationActions.AddAsync(action);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(RemediationAction action)
    {
        _dbContext.RemediationActions.Update(action);
        await _dbContext.SaveChangesAsync();
    }
}