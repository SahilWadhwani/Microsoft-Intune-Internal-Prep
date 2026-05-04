using EndpointGuardian.Api.Models;
using EndpointGuardian.Api.Repositories;

namespace EndpointGuardian.Api.Services;

public class EffectivePolicyResolver : IEffectivePolicyResolver
{
    private readonly IPolicyRepository _policyRepository;
    private readonly IPolicyAssignmentRepository _assignmentRepository;

    public EffectivePolicyResolver(
        IPolicyRepository policyRepository,
        IPolicyAssignmentRepository assignmentRepository)
    {
        _policyRepository = policyRepository;
        _assignmentRepository = assignmentRepository;
    }

    public async Task<List<CompliancePolicy>> ResolvePoliciesForDeviceAsync(ManagedDevice device)
    {
        var activeAssignments = (await _assignmentRepository.GetAllAsync())

        .Where(a => a.IsActive)

        .Where(a => AppliesToDevice(a, device))

        .ToList();

        var policies = new List<CompliancePolicy>();

        foreach (var assignment in activeAssignments)

        {   

            var policy = await _policyRepository.GetByIdAsync(assignment.PolicyId);

            if (policy is not null && policy.IsActive)

            {

                policies.Add(policy);

            }

        }

        return policies;
    }

    private static bool AppliesToDevice(PolicyAssignment assignment, ManagedDevice device)
    {
        if (assignment.TargetType == AssignmentTargetType.AllDevices)
        {
            return true;
        }

        if (assignment.TargetType == AssignmentTargetType.Device)
        {
            return assignment.TargetId == device.Id;
        }

        if (assignment.TargetType == AssignmentTargetType.Platform)
        {
            return string.Equals(
                assignment.TargetId,
                device.Platform.ToString(),
                StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }
}