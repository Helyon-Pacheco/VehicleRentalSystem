namespace VehicleRentalSystem.Messaging.Events;

public class CourierRegistered
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string CnhNumber { get; set; } = string.Empty;
    public string CnhType { get; set; } = string.Empty;
    public string? CnhImage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedByUser { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? UpdatedByUser { get; set; }
    public bool IsDeleted { get; set; } = false;
}
