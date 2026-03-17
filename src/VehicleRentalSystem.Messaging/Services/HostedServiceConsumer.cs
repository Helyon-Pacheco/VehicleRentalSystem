using VehicleRentalSystem.Core.Interfaces.Messaging;

namespace VehicleRentalSystem.Messaging.Services;

public class HostedServiceConsumer : BackgroundService
{
    private readonly IEnumerable<IMessageConsumer> _consumers;
    private readonly ILogger<HostedServiceConsumer> _logger;

    public HostedServiceConsumer(IEnumerable<IMessageConsumer> consumers, ILogger<HostedServiceConsumer> logger)
    {
        _consumers = consumers ?? throw new ArgumentNullException(nameof(consumers));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var tasks = _consumers.Select(async consumer =>
        {
            try
            {
                await consumer.ConsumeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while consuming messages.");
            }
        });

        await Task.WhenAll(tasks);
    }
}
