using VehicleRentalSystem.Core.Models.Enums;

namespace VehicleRentalSystem.RentalServices.Contracts.Request;

public class RentalUpdateRequest
{
    public Guid CourierId { get; init; } = Guid.Empty;
    public Guid VehicleId { get; init; } = Guid.Empty;
    public DateTime StartDate { get; init; } = DateTime.MinValue;
    public DateTime? EndDate { get; init; } = DateTime.MinValue;
    public DateTime ExpectedEndDate { get; init; } = DateTime.MinValue;
    public decimal DailyRate { get; init; } = 0m;
    public RentalPlan Plan { get; init; } = RentalPlan.SevenDays;
}
