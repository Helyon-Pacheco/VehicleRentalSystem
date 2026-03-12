using FluentValidation;
using FluentValidation.Results;
using VehicleRentalSystem.Core.Interfaces.UoW;

namespace VehicleRentalSystem.Core.Models.Validations;

public class CourierValidation : AbstractValidator<Courier>
{
    private readonly IUnitOfWork _unitOfWork;

    public CourierValidation(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public void ConfigureRulesForCreate()
    {
        RuleFor(c => c.Cnpj)
            .NotEmpty().WithMessage("The CNPJ cannot be empty.")
            .Length(14).WithMessage("The CNPJ must be 14 characters long.")
            .MustAsync(async (cnpj, cancellation) =>
            {
                var existingCourier = await _unitOfWork.Couriers.Find(c => c.Cnpj == cnpj);
                return !existingCourier.Any();
            }).WithMessage("The CNPJ already exists.");

        RuleFor(c => c.CnhNumber)
            .NotEmpty().WithMessage("The CNH Number cannot be empty.")
            .MustAsync(async (cnh, cancellation) =>
            {
                var existingCourier = await _unitOfWork.Couriers.Find(c => c.CnhNumber == cnh);
                return !existingCourier.Any();
            }).WithMessage("The CNH Number already exists.");

        ConfigureCommonRules();
    }

    public void ConfigureRulesForUpdate(Courier existingCourier)
    {
        RuleFor(c => c.Cnpj)
            .NotEmpty().WithMessage("The CNPJ cannot be empty.")
            .Length(14).WithMessage("The CNPJ must be 14 characters long.")
            .MustAsync(async (courier, cnpj, cancellation) =>
            {
                var existingCnpj = await _unitOfWork.Couriers.Find(c => c.Cnpj == cnpj);
                return !existingCnpj.Any() || existingCnpj.First().Id == courier.Id;
            }).WithMessage("The CNPJ already exists.")
            .When(c => existingCourier.Cnpj != c.Cnpj);

        RuleFor(c => c.CnhNumber)
            .NotEmpty().WithMessage("The CNH Number cannot be empty.")
            .MustAsync(async (courier, cnh, cancellation) =>
            {
                var existingCnh = await _unitOfWork.Couriers.Find(c => c.CnhNumber == cnh);
                return !existingCnh.Any() || existingCnh.First().Id == courier.Id;
            }).WithMessage("The CNH Number already exists.")
            .When(c => existingCourier.CnhNumber != c.CnhNumber);

        ConfigureCommonRules();
    }

    public void ConfigureCommonRules()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("The Name cannot be empty.")
            .Length(1, 100).WithMessage("The Name must be between 1 and 100 characters.");

        RuleFor(c => c.BirthDate)
            .NotEmpty().WithMessage("The Date of Birth cannot be empty.")
            .LessThan(DateOnly.FromDateTime(DateTime.Now.Date)).WithMessage("The Date of Birth must be in the past.");

        RuleFor(c => c.CnhType)
            .NotEmpty().WithMessage("The CNH Type cannot be empty.")
            .Must(type => type == "A" || type == "B" || type == "A+B").WithMessage("The CNH Type must be 'A', 'B', or 'A+B'.");
    }

    public async Task<ValidationResult> ValidateImageAsync(Courier courier)
    {
        var imageValidator = new InlineValidator<Courier>();
        imageValidator.RuleFor(c => c.CnhImage)
            .NotEmpty().WithMessage("The CNH Image cannot be empty.")
            .Must(image => image.EndsWith(".png") || image.EndsWith(".bmp")).WithMessage("The CNH Image must be a PNG or BMP file.");

        return await imageValidator.ValidateAsync(courier);
    }
}
