namespace AuctionSystem.Logic.Models.Vehicles;

public class Sedan(string manufacturer, int year, int startingBid, int numberOfDoors = 4) : 
    Vehicle(manufacturer, VehicleModels.Sedan, year, startingBid)
{
    public int NumberOfDoors { get; set; } = numberOfDoors;
}