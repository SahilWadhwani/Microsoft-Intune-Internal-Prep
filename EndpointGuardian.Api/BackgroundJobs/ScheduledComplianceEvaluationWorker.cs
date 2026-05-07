using EndpointGuardian.Api.Options;
using Microsoft.Extensions.Options;

namespace EndpointGuardian.Api.BackgroundJobs;

public class ScheduledComplianceEvaluationWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ScheduledComplianceEvaluationWorker> _logger;
    private readonly ScheduledComplianceOptions _options;

    public ScheduledComplianceEvaluationWorker(
        IServiceScopeFactory scopeFactory,
        IOptions<ScheduledComplianceOptions> options,
        ILogger<ScheduledComplianceEvaluationWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation("Scheduled compliance evaluation worker is disabled.");
            return;
        }

        _logger.LogInformation(
            "Scheduled compliance evaluation worker started with interval {IntervalSeconds} seconds.",
            _options.IntervalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var job = scope.ServiceProvider
                    .GetRequiredService<IScheduledComplianceEvaluationJob>();

                await job.RunAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Scheduled compliance evaluation worker is stopping.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while running scheduled compliance evaluation job.");
            }

            await Task.Delay(
                TimeSpan.FromSeconds(_options.IntervalSeconds),
                stoppingToken);
        }
    }
}