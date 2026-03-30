namespace AuctionSystem.Logic.Models.Vehicles;

public class SUV(string manufacturer, int year, int startingBid, int numberOfSeats = 5) : 
    Vehicle(manufacturer, VehicleModels.SUV, year, startingBid)
{
    public int NumberOfSeats { get; set; } = numberOfSeats;
}