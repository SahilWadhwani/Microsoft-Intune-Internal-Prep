public interface IComplianceEvaluator
{
       ComplianceEvaluationResult Evaluate(ManagedDevice device, CompliancePolicy policy); 
}