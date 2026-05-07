namespace EndpointGuardian.Api.BackgroundJobs;

public interface IScheduledComplianceEvaluationJob

{

    Task RunAsync(CancellationToken cancellationToken);

}