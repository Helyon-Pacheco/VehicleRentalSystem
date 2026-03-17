namespace VehicleRentalSystem.Core.Interfaces.Messaging;

public interface IMessageProducer
{
    Task PublishAsync<T>(T message, string exchange, string routingKey) where T : class;
}
