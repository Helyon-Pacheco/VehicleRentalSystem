namespace VehicleRentalSystem.Core.Dtos;

public class VehicleNotificationDto
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string CreatedByUser { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedByUser { get; set; }
    public bool IsDeleted { get; set; }
}
