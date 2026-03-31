namespace AuctionSystem.Logic.Models.Vehicles;

public class Hatchback(string plate, string manufacturer, string model, int year, int startingBid, int numberOfDoors = VehicleDefaults.DefaultHatchbackDoors) :
    Vehicle(plate, VehicleTypes.Hatchback, manufacturer, model, year, startingBid)
{
    public int NumberOfDoors { get; set; } = numberOfDoors;
    
}