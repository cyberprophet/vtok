namespace ShareInvest.Infrastructure;

public interface IScopedProcessingService
{
    Task DoWorkAsync(string key,
                     string authorization,
                     ICoreClient client,
                     CancellationToken stoppingToken);
}