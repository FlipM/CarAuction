namespace AuctionSystem.Logic.Models.Vehicles;

static class VehicleDefaults
{
    public const int DefaultHatchbackDoors = 4;
    public const int DefaultSedanDoors = 4;
    public const int DefaultNumberOfSeats = 5;
    public const int DefaultLoadCapacity = 1000;
}

    public const string Sedan = "SED
static class VehicleTypes
{
    public const string Sedan = "SED";
    public const string SUV = "SUV";
    public const string Truck = "TRK";
    public const string Hatchback = "HCB";
 
}

public abstract class Vehicle 
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; } = 0;
    public int StartingBid { get; set; } = 0;

    protected Vehicle(string type, string manufacturer, string model, int year, int startingBid)
    {
        Type = type;
        Manufacturer = manufacturer;
        Model = model;
        Year = year;
        StartingBid = startingBid;
    }
}