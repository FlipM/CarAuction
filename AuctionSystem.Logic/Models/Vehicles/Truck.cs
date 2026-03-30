namespace AuctionSystem.Logic.Models.Vehicles;

public class Truck(string manufacturer, int year, int startingBid, int loadCapacity = 1000) : 
    Vehicle(manufacturer, VehicleModels.Truck, year, startingBid)
{
    public int LoadCapacity { get; set; } = loadCapacity;

}