using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using VehicleRentalSystem.Core.Interfaces.Messaging;
using VehicleRentalSystem.Messaging.Events;

namespace VehicleRentalSystem.Messaging.Consumers;

public class CourierRegisteredConsumer : IMessageConsumer
{
    private readonly IChannel _channel;
    private readonly ILogger<CourierRegisteredConsumer> _logger;
    private readonly string _queueName = "courier_queue";

    public CourierRegisteredConsumer(IChannel channel, ILogger<CourierRegisteredConsumer> logger)
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
                var courierRegisteredEvent = JsonConvert.DeserializeObject<CourierRegistered>(message);

                if (courierRegisteredEvent is null)
                {
                    _logger.LogWarning("Received null or invalid message from queue '{QueueName}'. Discarding.", _queueName);
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, requeue: false);
                    return;
                }

                await ProcessMessageAsync(courierRegisteredEvent);
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

    private Task ProcessMessageAsync(CourierRegistered courierRegisteredEvent)
    {
        _logger.LogInformation("Processing CourierRegistered event: {Event}", courierRegisteredEvent);
        return Task.CompletedTask;
    }
}
