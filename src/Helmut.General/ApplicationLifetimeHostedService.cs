using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Helmut.General;

public class ApplicationLifetimeHostedService : IHostedService
{
    private readonly ILogger<ApplicationLifetimeHostedService> _logger;
    private readonly IHostApplicationLifetime _appLifetime;

    public ApplicationLifetimeHostedService(
        ILogger<ApplicationLifetimeHostedService> logger,
        IHostApplicationLifetime appLifetime)
    {
        _logger = logger;
        _appLifetime = appLifetime;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _appLifetime.ApplicationStarted.Register(OnStarted);
        _appLifetime.ApplicationStopping.Register(OnStopping);
        _appLifetime.ApplicationStopped.Register(OnStopped);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private void OnStarted()
    {
        _logger.LogInformation("OnStarted has been called.");

        _logger.LogInformation("The current time is: {CurrentTime}", DateTimeOffset.UtcNow);
    }

    private void OnStopping()
    {
        _logger.LogInformation("OnStopping has been called.");
    }

    private void OnStopped()
    {
        _logger.LogInformation("OnStopped has been called.");
    }
}
