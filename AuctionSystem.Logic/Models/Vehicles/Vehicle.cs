using System.ComponentModel.DataAnnotations;

namespace AuctionSystem.Logic.Models.Vehicles;

static class VehicleDefaults
{
    public const int DefaultHatchbackDoors = 4;
    public const int DefaultSedanDoors = 4;
    public const int DefaultNumberOfSeats = 5;
    public const int DefaultLoadCapacity = 1000;
}

static public class VehicleTypes
{
    public const string Sedan = "SED";
    public const string SUV = "SUV";
    public const string Truck = "TRK";
    public const string Hatchback = "HCB";
 
}

public abstract class Vehicle 
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [MaxLength(8), MinLength(1), RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Plate can only contain letters, numbers.")]
    public string Plate { get; set; } = string.Empty;

    [MaxLength(3), MinLength(3)]
    public string Type { get; set; } = string.Empty;
    
    [MaxLength(50), MinLength(2), RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Manufacturer can only contain letters, numbers, and spaces.")]
    public string Manufacturer { get; set; } = string.Empty;

    [MaxLength(50), MinLength(2), RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Model can only contain letters, numbers, and spaces.")]
    public string Model { get; set; } = string.Empty;

    [Range(1886, 2023, ErrorMessage = "Year must be a valid year.")]
    public int Year { get; set; } = 0;

    [Range(-1, int.MaxValue, ErrorMessage = "Starting bid must be a non-negative value.")]
    public int StartingBid { get; set; } = 0;

    protected Vehicle(string plate, string type, string manufacturer, string model, int year, int startingBid)
    {
        Plate = plate;
        Type = type;
        Manufacturer = manufacturer;
        Model = model;
        Year = year;
        StartingBid = startingBid;
    }
}