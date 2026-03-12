using FluentValidation;
using VehicleRentalSystem.Core.Interfaces.UoW;

namespace VehicleRentalSystem.Core.Models.Validations;

public class VehicleValidation : AbstractValidator<Vehicle>
{
    private readonly IUnitOfWork _unitOfWork;

    public VehicleValidation(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public void ConfigureRulesForCreate()
    {
        RuleFor(m => m.Plate)
            .NotEmpty().WithMessage("The Plate cannot be empty.")
            .Length(1, 10).WithMessage("The Plate must be between 1 and 10 characters.")
            .MustAsync(async (plate, cancellation) =>
            {
                var existingVehicle = await _unitOfWork.Vehicles.Find(m => m.Plate == plate);
                return !existingVehicle.Any();
            }).WithMessage("The Plate already exists.");

        ConfigureCommonRules();
    }

    public void ConfigureRulesForUpdate(Vehicle existingVehicle)
    {
        RuleFor(m => m.Plate)
            .NotEmpty().WithMessage("The Plate cannot be empty.")
            .Length(1, 10).WithMessage("The Plate must be between 1 and 10 characters.")
            .MustAsync(async (vehicle, plate, cancellation) =>
            {
                var existingPlate = await _unitOfWork.Vehicles.Find(m => m.Plate == plate);
                return !existingPlate.Any() || existingPlate.First().Id == vehicle.Id;
            }).WithMessage("The Plate already exists.")
            .When(m => existingVehicle.Plate != m.Plate);

        ConfigureCommonRules();
    }

    public void ConfigureCommonRules()
    {
        RuleFor(m => m.Year)
            .NotEmpty().WithMessage("The Year cannot be empty.")
            .InclusiveBetween(1900, DateTime.Now.Year + 1).WithMessage($"The Year must be between 1900 and {DateTime.Now.Year + 1}.");

        RuleFor(m => m.Model)
            .NotEmpty().WithMessage("The Model cannot be empty.")
            .Length(1, 50).WithMessage("The Model must be between 1 and 50 characters.");
    }
}
