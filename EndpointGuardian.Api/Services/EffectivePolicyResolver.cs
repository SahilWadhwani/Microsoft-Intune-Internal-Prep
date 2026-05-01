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

    public List<CompliancePolicy> ResolvePoliciesForDevice(ManagedDevice device)
    {
        var activeAssignments = _assignmentRepository
            .GetAll()
            .Where(a => a.IsActive)
            .Where(a => AppliesToDevice(a, device))
            .ToList();

        var policies = activeAssignments
            .Select(a => _policyRepository.GetById(a.PolicyId))
            .Where(p => p is not null && p.IsActive)
            .Select(p => p!)
            .ToList();

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