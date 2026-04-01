using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using VehicleRentalSystem.Core.Interfaces.Messaging;
using VehicleRentalSystem.Messaging.Events;

namespace VehicleRentalSystem.Messaging.Consumers;

public class RentalRegisteredConsumer : IMessageConsumer
{
    private readonly IChannel _channel;
    private readonly ILogger<RentalRegisteredConsumer> _logger;
    private readonly string _queueName = "rental_queue";

    public RentalRegisteredConsumer(IChannel channel, ILogger<RentalRegisteredConsumer> logger)
    {
        _channel = channel ?? throw new ArgumentNullException(nameof(channel));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task ConsumeAsync()
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var rentalRegisteredEvent = JsonConvert.DeserializeObject<RentalRegistered>(message);

                if (rentalRegisteredEvent is null)
                {
                    _logger.LogWarning("Received null or invalid message from queue '{QueueName}'. Discarding.", _queueName);
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, requeue: false);
                    return;
                }

                await ProcessMessageAsync(rentalRegisteredEvent);
                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
                await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
            }
        };

        await _channel.BasicConsumeAsync(_queueName, false, consumer);
        await Task.CompletedTask;
    }

    private Task ProcessMessageAsync(RentalRegistered rentalRegisteredEvent)
    {
        _logger.LogInformation("Processing RentalRegistered event: {Event}", rentalRegisteredEvent);
        return Task.CompletedTask;
    }
}
