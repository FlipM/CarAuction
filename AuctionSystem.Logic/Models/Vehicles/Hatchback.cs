namespace AuctionSystem.Logic.Models.Vehicles;

public class Hatchback(string manufacturer, int year, int startingBid, int numberOfDoors = 4) :
    Vehicle(manufacturer, VehicleModels.Hatchback, year, startingBid)
{
    public int NumberOfDoors { get; set; } = numberOfDoors;
    
}