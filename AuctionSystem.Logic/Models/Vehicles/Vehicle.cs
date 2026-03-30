namespace AuctionSystem.Logic.Models.Vehicles;

static class VehicleModels
{
    public const string Sedan = "SED";
    public const string SUV = "SUV";
    public const string Truck = "TRK";
    public const string Hatchback = "HCB";
 
}

public abstract class Vehicle 
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Manufacturer { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public int StartingBid { get; set; } = 0;

    protected Vehicle(string manufacturer, string model, int year, int startingBid)
    {
        Manufacturer = manufacturer;
        Model = model;
        Year = year;
        StartingBid = startingBid;
    }
}