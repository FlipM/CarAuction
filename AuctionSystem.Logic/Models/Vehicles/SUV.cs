namespace AuctionSystem.Logic.Models.Vehicles;

public class SUV(string plate, string manufacturer, string model, int year, int startingBid, int numberOfSeats = VehicleDefaults.DefaultNumberOfSeats) : 
    Vehicle(plate, VehicleTypes.SUV, manufacturer, model, year, startingBid)
{
    public int NumberOfSeats { get; set; } = numberOfSeats;
}