using EndpointGuardian.Api.Models;

namespace EndpointGuardian.Api.Services;

public interface IPolicyService
{
    PolicyResponse? CreatePolicy(CreatePolicyRequest request);
    PolicyResponse? GetPolicyById(string id);
    PagedPoliciesResponse GetPolicies(GetPoliciesQuery query);
}