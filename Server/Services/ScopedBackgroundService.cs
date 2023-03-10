using ShareInvest.Infrastructure;
using ShareInvest.Infrastructure.Http;

namespace ShareInvest.Server.Services;

public class ScopedBackgroundService : BackgroundService
{
    public ScopedBackgroundService(IServiceProvider provider,
                                   IConfiguration configuration,
                                   ILogger<ScopedBackgroundService> logger)
    {
        this.provider = provider;
        this.logger = logger;

        authorization = string.Concat(Properties.Resources.AK,
                                      ' ',
                                      configuration["KakaoTalk:ClientId"]);

        key = configuration[nameof(Properties.Resources.DART)];
    }
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("{ } is starting at { }.",
                              nameof(ScopedBackgroundService),
                              DateTime.Now.ToString("G"));

        await base.StartAsync(cancellationToken);
    }
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogWarning("{ } is stopping at { }.",
                          nameof(ScopedBackgroundService),
                          DateTime.Now.ToString("G"));

        await base.StopAsync(cancellationToken);
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        do
        {
            logger.LogWarning("{ } is executed at { }.",
                              nameof(ScopedBackgroundService),
                              DateTime.Now.ToString("G"));

            using (var scope = provider.CreateScope())
            {
                var service = scope.ServiceProvider
                                   .GetRequiredService<IScopedProcessingService>();

                await service.DoWorkAsync(key,
                                          authorization,
                                          new CoreRestClient(Properties.Resources.DART),
                                          stoppingToken);
            }
            await Task.Delay(0x200 * 0x200,
                             stoppingToken);
        }
        while (stoppingToken.IsCancellationRequested is false);
    }
    readonly string key;
    readonly string authorization;
    readonly IServiceProvider provider;
    readonly ILogger<ScopedBackgroundService> logger;
}