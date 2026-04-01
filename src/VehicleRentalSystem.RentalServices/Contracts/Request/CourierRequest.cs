namespace VehicleRentalSystem.RentalServices.Contracts.Request;

public class CourierRequest
{
    public string Name { get; init; } = string.Empty;
    public string Cnpj { get; init; } = string.Empty;
    public DateOnly BirthDate { get; init; } = DateOnly.MinValue;
    public string CnhNumber { get; init; } = string.Empty;
    public string CnhType { get; init; } = string.Empty;
}
