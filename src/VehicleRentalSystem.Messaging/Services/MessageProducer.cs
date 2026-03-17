using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using VehicleRentalSystem.Core.Interfaces.Messaging;

namespace VehicleRentalSystem.Messaging.Services;

public class MessageProducer : IMessageProducer
{
    private readonly IChannel _channel;
    private readonly ILogger<MessageProducer> _logger;

    public MessageProducer(IChannel channel, ILogger<MessageProducer> logger)
    {
        _channel = channel ?? throw new ArgumentNullException(nameof(channel));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task PublishAsync<T>(T message, string exchange, string routingKey) where T : class
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        try
        {
            var messageBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            await _channel.BasicPublishAsync(exchange, routingKey, messageBody);
            _logger.LogInformation("Message published to exchange {Exchange} with routing key {RoutingKey}", exchange, routingKey);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing message to exchange {Exchange} with routing key {RoutingKey}", exchange, routingKey);
            throw;
        }
    }
}
