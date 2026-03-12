using VehicleRentalSystem.Core.Models.Enums;

namespace VehicleRentalSystem.Core.Models;

public class Vehicle : EntityBase
{
    public int Year { get; set; }
    public string Model { get; set; } = string.Empty;
    public string Plate { get; set; } = string.Empty;
    public VehicleType VehicleType {  get; set; }

    public virtual VehicleNotification? MotorcycleNotification { get; set; }

    public Vehicle()
    {
    }

    public Vehicle(int year, string model, string plate, VehicleType vehicleType)
    {
        Year = year;
        Model = model;
        Plate = plate;
        VehicleType = vehicleType;
    }
}
