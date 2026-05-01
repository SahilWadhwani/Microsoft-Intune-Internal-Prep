public interface IEffectivePolicyResolver
{
    List<CompliancePolicy> ResolvePoliciesForDevice(ManagedDevice device);
}