using EndpointGuardian.Api.Data;
using EndpointGuardian.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EndpointGuardian.Api.Repositories;

public class EfPolicyAssignmentRepository : IPolicyAssignmentRepository
{
    private readonly EndpointGuardianDbContext _dbContext;

    public EfPolicyAssignmentRepository(EndpointGuardianDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<PolicyAssignment>> GetAllAsync()
    {
        return await _dbContext.PolicyAssignments.ToListAsync();
    }

    public async Task<List<PolicyAssignment>> GetByPolicyIdAsync(string policyId)
    {
        return await _dbContext.PolicyAssignments
            .Where(a => a.PolicyId == policyId)
            .ToListAsync();
    }

    public async Task<PolicyAssignment?> GetByIdAsync(string assignmentId)
    {
        return await _dbContext.PolicyAssignments
            .FirstOrDefaultAsync(a => a.Id == assignmentId);
    }

    public async Task AddAsync(PolicyAssignment assignment)
    {
        await _dbContext.PolicyAssignments.AddAsync(assignment);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(PolicyAssignment assignment)
    {
        _dbContext.PolicyAssignments.Update(assignment);
        await _dbContext.SaveChangesAsync();
    }
}