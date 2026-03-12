namespace VehicleRentalSystem.Core.Dtos;

public class VehicleDto
{
    public Guid Id { get; set; }
    public int Year { get; set; }
    public string Model { get; set; } = string.Empty;
    public string Plate { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string CreatedByUser { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedByUser { get; set; }
    public bool IsDeleted { get; set; }

    public VehicleNotificationDto VehicleNotification { get; set; } = new();
}
