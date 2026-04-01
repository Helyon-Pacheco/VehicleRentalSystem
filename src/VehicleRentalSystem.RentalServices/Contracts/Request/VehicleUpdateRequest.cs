using VehicleRentalSystem.Core.Models.Enums;

namespace VehicleRentalSystem.RentalServices.Contracts.Request;

public class VehicleUpdateRequest
{
    public int Year { get; init; } = 0;
    public string Model { get; init; } = string.Empty;
    public string Plate { get; init; } = string.Empty;
    public VehicleType VehicleType { get; init; }
}
