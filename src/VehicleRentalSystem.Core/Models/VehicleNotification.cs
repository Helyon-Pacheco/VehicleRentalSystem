namespace VehicleRentalSystem.Core.Models;

public class VehicleNotification : EntityBase
{
    public Guid VehicleId { get; set; }
    public string Message { get; set; } = string.Empty;

    public virtual Vehicle Vehicle { get; set; } = null!;
}
