using EndpointGuardian.Api.Models;

namespace EndpointGuardian.Api.Repositories;

public interface IPolicyAssignmentRepository

{

    List<PolicyAssignment> GetAll();

    List<PolicyAssignment> GetByPolicyId(string policyId);

    PolicyAssignment? GetById(string assignmentId);

    void Add(PolicyAssignment assignment);

}