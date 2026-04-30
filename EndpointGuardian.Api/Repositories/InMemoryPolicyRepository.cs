using EndpointGuardian.Api.Models;

namespace EndpointGuardian.Api.Repositories;

public class InMemoryPolicyRepository : IPolicyRepository
{
    private readonly List<CompliancePolicy> _policies = new();

    public List<CompliancePolicy> GetAll()
    {
        return _policies;
    }

    public CompliancePolicy? GetById(string id)
    {
        return _policies.FirstOrDefault(p => p.Id == id);
    }

    public void Add(CompliancePolicy policy)
    {
        _policies.Add(policy);
    }
}