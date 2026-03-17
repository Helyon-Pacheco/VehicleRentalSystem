using RabbitMQ.Client;

namespace VehicleRentalSystem.Messaging.Services;

public class SetupHostedService : IHostedService
{
    private readonly IConnection _connection;
    private readonly ILogger<SetupHostedService> _logger;

    public SetupHostedService(IConnection connection, ILogger<SetupHostedService> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var channel = await _connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync("courier_exchange", ExchangeType.Direct, true);
            await channel.ExchangeDeclareAsync("vehicle_exchange", ExchangeType.Direct, true);
            await channel.ExchangeDeclareAsync("rental_exchange", ExchangeType.Direct, true);

            await channel.QueueDeclareAsync("courier_queue", true, false, false, null);
            await channel.QueueDeclareAsync("vehicle_queue", true, false, false, null);
            await channel.QueueDeclareAsync("rental_queue", true, false, false, null);

            await channel.QueueBindAsync("courier_queue", "courier_exchange", "courier_routingKey");
            await channel.QueueBindAsync("vehicle_queue", "vehicle_exchange", "vehicle_routingKey");
            await channel.QueueBindAsync("rental_queue", "rental_exchange", "rental_routingKey");

            _logger.LogInformation("RabbitMQ setup completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to setup RabbitMQ exchanges/queues.");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
