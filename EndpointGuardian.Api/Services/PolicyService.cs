using EndpointGuardian.Api.Models;
using EndpointGuardian.Api.Repositories;

namespace EndpointGuardian.Api.Services;

public class PolicyService : IPolicyService
{
    private readonly IPolicyRepository _policyRepository;
    private readonly ILogger<PolicyService> _logger;

    public PolicyService(
        IPolicyRepository policyRepository,
        ILogger<PolicyService> logger)
    {
        _policyRepository = policyRepository;
        _logger = logger;
    }

    public PolicyResponse? CreatePolicy(CreatePolicyRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            _logger.LogWarning("Policy creation rejected because name was empty");
            return null;
        }

        if (request.MinimumOsVersion <= 0)
        {
            _logger.LogWarning(
                "Policy creation rejected because minimum OS version {MinimumOsVersion} was invalid",
                request.MinimumOsVersion);

            return null;
        }

        if (request.MaxCheckInAgeHours <= 0)
        {
            _logger.LogWarning(
                "Policy creation rejected because max check-in age {MaxCheckInAgeHours} was invalid",
                request.MaxCheckInAgeHours);

            return null;
        }

        var policy = new CompliancePolicy(
            Guid.NewGuid().ToString(),
            request.Name,
            request.MinimumOsVersion,
            request.RequireEncryption,
            request.RequirePassword,
            request.RequireDefender,
            request.MaxCheckInAgeHours
        );

        _policyRepository.Add(policy);

        _logger.LogInformation(
            "Policy {PolicyId} created successfully with version {PolicyVersion}",
            policy.Id,
            policy.Version);

        return ToPolicyResponse(policy);
    }

    public PolicyResponse? GetPolicyById(string id)
    {
        var policy = _policyRepository.GetById(id);

        if (policy is null)
        {
            _logger.LogWarning("Policy {PolicyId} was not found", id);
            return null;
        }

        return ToPolicyResponse(policy);
    }

    public PagedPoliciesResponse GetPolicies(GetPoliciesQuery query)
    {
        var policies = _policyRepository.GetAll().AsEnumerable();

        if (query.IsActive is not null)
        {
            policies = policies.Where(p => p.IsActive == query.IsActive);
        }

        var totalCount = policies.Count();

        var items = policies
            .OrderBy(p => p.Name)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(ToPolicySummaryResponse)
            .ToList();

        return new PagedPoliciesResponse(
            items,
            query.Page,
            query.PageSize,
            totalCount
        );
    }

    private static PolicyResponse ToPolicyResponse(CompliancePolicy policy)
    {
        return new PolicyResponse(
            policy.Id,
            policy.Name,
            policy.Version,
            policy.MinimumOsVersion,
            policy.RequireEncryption,
            policy.RequirePassword,
            policy.RequireDefender,
            policy.MaxCheckInAgeHours,
            policy.IsActive,
            policy.CreatedAtUtc
        );
    }

    private static PolicySummaryResponse ToPolicySummaryResponse(CompliancePolicy policy)
    {
        return new PolicySummaryResponse(
            policy.Id,
            policy.Name,
            policy.Version,
            policy.IsActive,
            policy.CreatedAtUtc
        );
    }
}