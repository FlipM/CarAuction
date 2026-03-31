namespace AuctionSystem.Logic.Models.Vehicles;

public class Truck(string plate, string manufacturer, string model, int year, int startingBid, int loadCapacity = VehicleDefaults.DefaultLoadCapacity) : 
    Vehicle(plate, VehicleTypes.Truck, manufacturer, model, year, startingBid)
{
    public int LoadCapacity { get; set; } = loadCapacity;

}