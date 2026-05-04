using EndpointGuardian.Api.Models;

namespace EndpointGuardian.Api.Repositories;

public interface IPolicyAssignmentRepository

{

    Task<List<PolicyAssignment>> GetAllAsync();

    Task<List<PolicyAssignment>> GetByPolicyIdAsync(string policyId);

    Task<PolicyAssignment?> GetByIdAsync(string assignmentId);

    Task AddAsync(PolicyAssignment assignment);

    Task UpdateAsync(PolicyAssignment assignment);

}