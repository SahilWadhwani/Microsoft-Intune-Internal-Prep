using EndpointGuardian.Api.Models;
using EndpointGuardian.Api.Repositories;

namespace EndpointGuardian.Api.Services;

public class PolicyAssignmentService : IPolicyAssignmentService
{
    private readonly IPolicyRepository _policyRepository;
    private readonly IDeviceRepository _deviceRepository;
    private readonly IPolicyAssignmentRepository _assignmentRepository;
    private readonly ILogger<PolicyAssignmentService> _logger;

    public PolicyAssignmentService(
        IPolicyRepository policyRepository,
        IDeviceRepository deviceRepository,
        IPolicyAssignmentRepository assignmentRepository,
        ILogger<PolicyAssignmentService> logger)
    {
        _policyRepository = policyRepository;
        _deviceRepository = deviceRepository;
        _assignmentRepository = assignmentRepository;
        _logger = logger;
    }

    public PolicyAssignmentResponse? CreateAssignment(
        string policyId,
        CreatePolicyAssignmentRequest request)
    {
        var policy = _policyRepository.GetById(policyId);

        if (policy is null)
        {
            _logger.LogWarning(
                "Cannot create assignment because policy {PolicyId} was not found",
                policyId);

            return null;
        }

        if (!policy.IsActive)
        {
            _logger.LogWarning(
                "Cannot create assignment because policy {PolicyId} is inactive",
                policyId);

            return null;
        }

        if (!IsTargetValid(request.TargetType, request.TargetId))
        {
            _logger.LogWarning(
                "Cannot create assignment for policy {PolicyId} because target {TargetType}:{TargetId} is invalid",
                policyId,
                request.TargetType,
                request.TargetId);

            return null;
        }

        var duplicateExists = _assignmentRepository
            .GetAll()
            .Any(a =>
                a.IsActive &&
                a.PolicyId == policyId &&
                a.TargetType == request.TargetType &&
                a.TargetId == request.TargetId);

        if (duplicateExists)
        {
            _logger.LogWarning(
                "Duplicate active assignment rejected for policy {PolicyId}, target {TargetType}:{TargetId}",
                policyId,
                request.TargetType,
                request.TargetId);

            return null;
        }

        var assignment = new PolicyAssignment(
            Guid.NewGuid().ToString(),
            policyId,
            request.TargetType,
            request.TargetId,
            request.AssignedBy
        );

        _assignmentRepository.Add(assignment);

        _logger.LogInformation(
            "Policy {PolicyId} assigned to {TargetType}:{TargetId} by {AssignedBy}",
            policyId,
            assignment.TargetType,
            assignment.TargetId,
            assignment.AssignedBy);

        return ToResponse(assignment);
    }

    public PagedPolicyAssignmentsResponse? GetAssignmentsForPolicy(
        string policyId,
        GetPolicyAssignmentsQuery query)
    {
        var policy = _policyRepository.GetById(policyId);

        if (policy is null)
        {
            _logger.LogWarning(
                "Cannot list assignments because policy {PolicyId} was not found",
                policyId);

            return null;
        }

        var assignments = _assignmentRepository
            .GetByPolicyId(policyId)
            .AsEnumerable();

        if (query.IsActive is not null)
        {
            assignments = assignments.Where(a => a.IsActive == query.IsActive);
        }

        var totalCount = assignments.Count();

        var items = assignments
            .OrderByDescending(a => a.AssignedAtUtc)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(ToResponse)
            .ToList();

        return new PagedPolicyAssignmentsResponse(
            items,
            query.Page,
            query.PageSize,
            totalCount);
    }

    public PolicyAssignmentResponse? GetAssignmentById(string assignmentId)
    {
        var assignment = _assignmentRepository.GetById(assignmentId);

        if (assignment is null)
        {
            _logger.LogWarning(
                "Assignment {AssignmentId} was not found",
                assignmentId);

            return null;
        }

        return ToResponse(assignment);
    }

    private bool IsTargetValid(AssignmentTargetType targetType, string? targetId)
    {
        if (targetType == AssignmentTargetType.AllDevices)
        {
            return targetId is null;
        }

        if (string.IsNullOrWhiteSpace(targetId))
        {
            return false;
        }

        if (targetType == AssignmentTargetType.Device)
        {
            return _deviceRepository.GetById(targetId) is not null;
        }

        if (targetType == AssignmentTargetType.Platform)
        {
            return Enum.TryParse<DevicePlatform>(
                targetId,
                ignoreCase: true,
                out _);
        }

        return false;
    }

    private static PolicyAssignmentResponse ToResponse(PolicyAssignment assignment)
    {
        return new PolicyAssignmentResponse(
            assignment.Id,
            assignment.PolicyId,
            assignment.TargetType,
            assignment.TargetId,
            assignment.AssignedAtUtc,
            assignment.AssignedBy,
            assignment.IsActive);
    }
}