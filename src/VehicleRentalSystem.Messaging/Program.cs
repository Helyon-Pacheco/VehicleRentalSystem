using VehicleRentalSystem.Messaging.Configurations;

var builder = Host.CreateApplicationBuilder(args);

var messagingSettings = new MessagingSettings();
messagingSettings.ConfigureServices(builder.Services, builder.Configuration);

var host = builder.Build();

host.Run();
