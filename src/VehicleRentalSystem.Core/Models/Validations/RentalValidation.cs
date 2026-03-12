using FluentValidation;
using VehicleRentalSystem.Core.Interfaces.UoW;

namespace VehicleRentalSystem.Core.Models.Validations;

public class RentalValidation : AbstractValidator<Rental>
{
    private readonly IUnitOfWork _unitOfWork;

    public RentalValidation(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        ConfigureCommonRules();
    }

    public void ConfigureCommonRules()
    {
        RuleFor(r => r.CourierId)
            .NotEmpty().WithMessage("The Courier ID cannot be empty.");

        RuleFor(r => r.VehicleId)
            .NotEmpty().WithMessage("The Motorcycle ID cannot be empty.");

        RuleFor(r => r.StartDate)
            .NotEmpty().WithMessage("The Start Date cannot be empty.")
            .GreaterThan(DateTime.UtcNow).WithMessage("The Start Date must be in the future.");

        RuleFor(r => r.EndDate)
            .NotEmpty().WithMessage("The End Date cannot be empty.")
            .GreaterThan(r => r.StartDate).WithMessage("The End Date must be after the Start Date.");

        RuleFor(r => r.ExpectedEndDate)
            .NotEmpty().WithMessage("The Expected End Date cannot be empty.")
            .GreaterThan(r => r.StartDate).WithMessage("The Expected End Date must be after the Start Date.");

        RuleFor(r => r.DailyRate)
            .GreaterThan(0).WithMessage("The Daily Rate must be a positive number.");

        RuleFor(r => r.Plan)
            .IsInEnum().WithMessage("The Plan is not valid.");

        RuleFor(r => r.TotalCost)
            .GreaterThanOrEqualTo(0).WithMessage("The Total Cost must be a positive number.");
    }
}
