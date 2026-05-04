using EndpointGuardian.Api.Data;
using EndpointGuardian.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EndpointGuardian.Api.Repositories;

public class EfPolicyRepository : IPolicyRepository
{
    private readonly EndpointGuardianDbContext _dbContext;

    public EfPolicyRepository(EndpointGuardianDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<CompliancePolicy>> GetAllAsync()
    {
        return await _dbContext.Policies.ToListAsync();
    }

    public async Task<CompliancePolicy?> GetByIdAsync(string id)
    {
        return await _dbContext.Policies
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AddAsync(CompliancePolicy policy)
    {
        await _dbContext.Policies.AddAsync(policy);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(CompliancePolicy policy)
    {
        _dbContext.Policies.Update(policy);
        await _dbContext.SaveChangesAsync();
    }
}