using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using VehicleRentalSystem.Core.Interfaces.Messaging;
using VehicleRentalSystem.Core.Interfaces.Services;
using VehicleRentalSystem.Core.Interfaces.UoW;
using VehicleRentalSystem.Core.Models;
using VehicleRentalSystem.Messaging.Events;

namespace VehicleRentalSystem.Messaging.Consumers;

public class VehicleRegisteredConsumer : IMessageConsumer
{
    private readonly IChannel _channel;
    private readonly ILogger<VehicleRegisteredConsumer> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly string _queueName = "motorcycle_queue";

    public VehicleRegisteredConsumer(IChannel channel, ILogger<VehicleRegisteredConsumer> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _channel = channel ?? throw new ArgumentNullException(nameof(channel));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
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
                var motorcycleRegisteredEvent = JsonConvert.DeserializeObject<VehicleRegistered>(message);

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var cacheService = scope.ServiceProvider.GetRequiredService<IRedisCacheService>();
                    await ProcessMessageAsync(motorcycleRegisteredEvent, unitOfWork, cacheService);
                }

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

    private async Task ProcessMessageAsync(VehicleRegistered vehicleRegisteredEvent, IUnitOfWork unitOfWork, IRedisCacheService cacheService)
    {
        _logger.LogInformation("Processing VehicleRegistered event: {Event}", vehicleRegisteredEvent);

        if (vehicleRegisteredEvent.Year == 2024)
        {
            var existingNotification = await unitOfWork.VehicleNotifications
                .Find(mn => mn.VehicleId == vehicleRegisteredEvent.Id && mn.Message == "Vehicle of year 2024 registered.");

            if (existingNotification.Any())
            {
                _logger.LogInformation("Notification already exists for Vehicle ID: {VehicleId}", vehicleRegisteredEvent.Id);
                return;
            }

            var notification = new VehicleNotification
            {
                VehicleId = vehicleRegisteredEvent.Id,
                Message = "Vehicle of year 2024 registered.",
                CreatedAt = DateTime.UtcNow,
                CreatedByUser = vehicleRegisteredEvent.CreatedByUser
            };

            await unitOfWork.VehicleNotifications.Add(notification);
            await unitOfWork.SaveAsync();

            var cacheKey = $"VehicleNotification:{notification.VehicleId}";
            await cacheService.RemoveCacheValueAsync(cacheKey);
            await cacheService.RemoveCacheValueAsync("VehicleList:All");
        }
    }
}
