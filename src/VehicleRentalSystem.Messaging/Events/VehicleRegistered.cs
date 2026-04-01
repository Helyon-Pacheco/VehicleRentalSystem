using VehicleRentalSystem.Core.Models.Enums;

namespace VehicleRentalSystem.Messaging.Events;

public class VehicleRegistered
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int Year { get; set; }
    public string Model { get; set; } = string.Empty;
    public string Plate { get; set; } = string.Empty;
    public VehicleType VehicleType { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedByUser { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? UpdatedByUser { get; set; }
    public bool IsDeleted { get; set; } = false;
}
