namespace AuctionSystem.Logic.Models.Vehicles;

public class Hatchback(string manufacturer, string model, int year, int startingBid, int numberOfDoors = VehicleDefaults.DefaultHatchbackDoors) :
    Vehicle(VehicleTypes.Hatchback, manufacturer, model, year, startingBid)
{
    public int NumberOfDoors { get; set; } = numberOfDoors;
    
}