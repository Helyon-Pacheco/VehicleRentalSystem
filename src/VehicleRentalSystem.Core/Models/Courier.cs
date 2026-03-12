namespace VehicleRentalSystem.Core.Models;

public class Courier : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string CnhNumber { get; set; } = string.Empty;
    public string CnhType { get; set; } = string.Empty;
    public string? CnhImage { get; set; }

    public Courier() { }

    public Courier(string name, string cnpj, DateOnly birthDate, string cnhNumber, string cnhType, string? cnhImage)
    {
        Name = name;
        Cnpj = cnpj;
        BirthDate = birthDate;
        CnhNumber = cnhNumber;
        CnhType = cnhType;
        CnhImage = cnhImage;
    }
}
