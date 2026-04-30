using EndpointGuardian.Api.Models;

namespace EndpointGuardian.Api.Repositories;

public class InMemoryPolicyAssignmentRepository : IPolicyAssignmentRepository
{
    private readonly List<PolicyAssignment> _assignments = new();

    public List<PolicyAssignment> GetAll()
    {
        return _assignments;
    }

    public List<PolicyAssignment> GetByPolicyId(string policyId)
    {
        return _assignments
            .Where(a => a.PolicyId == policyId)
            .ToList();
    }

    public PolicyAssignment? GetById(string assignmentId)
    {
        return _assignments.FirstOrDefault(a => a.Id == assignmentId);
    }

    public void Add(PolicyAssignment assignment)
    {
        _assignments.Add(assignment);
    }
}