using EndpointGuardian.Api.Models;

namespace EndpointGuardian.Api.Repositories;

public interface IAuditEventRepository

{

    Task AddAsync(AuditEvent auditEvent);

    Task<List<AuditEvent>> GetAllAsync();

}