namespace VehicleRentalSystem.Core.Dtos;

public class CourierDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string CnhNumber { get; set; } = string.Empty;
    public string CnhType { get; set; } = string.Empty;
    public string? CnhImage { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedByUser { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedByUser { get; set; }
    public bool IsDeleted { get; set; }
}
