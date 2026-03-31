namespace AuctionSystem.Logic.Models.Vehicles;

public class Truck(string manufacturer, string model, int year, int startingBid, int loadCapacity = VehicleDefaults.DefaultLoadCapacity) : 
    Vehicle(VehicleTypes.Truck, manufacturer, model, year, startingBid)
{
    public int LoadCapacity { get; set; } = loadCapacity;

}