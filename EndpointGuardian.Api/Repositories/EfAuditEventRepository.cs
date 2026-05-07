using EndpointGuardian.Api.Data;
using EndpointGuardian.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EndpointGuardian.Api.Repositories;

public class EfAuditEventRepository : IAuditEventRepository
{
    private readonly EndpointGuardianDbContext _dbContext;

    public EfAuditEventRepository(EndpointGuardianDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(AuditEvent auditEvent)
    {
        await _dbContext.AuditEvents.AddAsync(auditEvent);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<AuditEvent>> GetAllAsync()
    {
        return await _dbContext.AuditEvents.ToListAsync();
    }
}