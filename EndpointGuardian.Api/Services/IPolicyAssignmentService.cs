using EndpointGuardian.Api.Models;

namespace EndpointGuardian.Api.Services;

public interface IPolicyAssignmentService
{
    PolicyAssignmentResponse? CreateAssignment(
        string policyId,
        CreatePolicyAssignmentRequest request);

    PagedPolicyAssignmentsResponse? GetAssignmentsForPolicy(
        string policyId,
        GetPolicyAssignmentsQuery query);

    PolicyAssignmentResponse? GetAssignmentById(string assignmentId);
}