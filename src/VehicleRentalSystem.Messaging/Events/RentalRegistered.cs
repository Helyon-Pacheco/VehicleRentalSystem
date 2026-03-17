using VehicleRentalSystem.Core.Models.Enums;

namespace VehicleRentalSystem.Messaging.Events;

public class RentalRegistered
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CourierId { get; set; }
    public Guid VehicleId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime ExpectedEndDate { get; set; }
    public decimal DailyRate { get; set; }
    public decimal TotalCost { get; set; }
    public RentalPlan Plan { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedByUser { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? UpdatedByUser { get; set; }
    public bool IsDeleted { get; set; } = false;
}
