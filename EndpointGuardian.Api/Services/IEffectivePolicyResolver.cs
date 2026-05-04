public interface IEffectivePolicyResolver
{
    Task<List<CompliancePolicy>> ResolvePoliciesForDeviceAsync(ManagedDevice device);
}