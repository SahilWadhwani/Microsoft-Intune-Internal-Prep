using EndpointGuardian.Api.Models;

namespace EndpointGuardian.Api.Services;

public interface IComplianceEvaluator
{
    PolicyEvaluationResult EvaluatePolicy(
        ManagedDevice device,
        CompliancePolicy policy);
}