namespace AuctionSystem.Logic.Models.Vehicles;

public class Sedan(string manufacturer, string model, int year, int startingBid, int numberOfDoors = VehicleDefaults.DefaultSedanDoors) : 
    Vehicle(VehicleTypes.Sedan, manufacturer, model, year, startingBid)
{
    public int NumberOfDoors { get; set; } = numberOfDoors;
}