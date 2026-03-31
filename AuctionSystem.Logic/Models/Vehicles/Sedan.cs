namespace AuctionSystem.Logic.Models.Vehicles;

public class Sedan(string plate, string manufacturer, string model, int year, int startingBid, int numberOfDoors = VehicleDefaults.DefaultSedanDoors) : 
    Vehicle(plate, VehicleTypes.Sedan, manufacturer, model, year, startingBid)
{
    public int NumberOfDoors { get; set; } = numberOfDoors;
}