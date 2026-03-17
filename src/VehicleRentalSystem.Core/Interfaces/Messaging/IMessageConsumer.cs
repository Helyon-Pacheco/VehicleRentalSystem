namespace VehicleRentalSystem.Core.Interfaces.Messaging;

public interface IMessageConsumer
{
    Task ConsumeAsync();
}
