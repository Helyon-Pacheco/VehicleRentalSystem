using VehicleRentalSystem.Core.Models.Enums;

namespace VehicleRentalSystem.Core.Models;

public class Rental : EntityBase
{
    public Guid CourierId { get; set; }
    public Guid VehicleId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime ExpectedEndDate { get; set; }
    public decimal DailyRate { get; set; }
    public decimal TotalCost { get; set; }
    public RentalPlan Plan { get; set; }

    public virtual Courier Courier { get; set; }
    public virtual Vehicle Vehicle { get; set; }

    public Rental()
    {
    }

    public Rental(Guid courierId, Guid vehicleId, DateTime startDate, DateTime endDate, DateTime expectedEndDate, decimal dailyRate, RentalPlan plan)
    {
        CourierId = courierId;
        VehicleId = vehicleId;
        StartDate = startDate;
        EndDate = endDate;
        ExpectedEndDate = expectedEndDate;
        DailyRate = dailyRate;
        Plan = plan;
        TotalCost = CalculateTotalCost();
    }

    private decimal CalculateTotalCost()
    {
        return Plan switch
        {
            RentalPlan.SevenDays => 7 * DailyRate,
            RentalPlan.FifteenDays => 15 * DailyRate,
            RentalPlan.ThirtyDays => 30 * DailyRate,
            RentalPlan.FortyFiveDays => 45 * DailyRate,
            RentalPlan.FiftyDays => 50 * DailyRate,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
