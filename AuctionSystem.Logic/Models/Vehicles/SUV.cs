namespace AuctionSystem.Logic.Models.Vehicles;
using System.ComponentModel.DataAnnotations;

public class SUV(string plate, string manufacturer, string model, int year, int startingBid, int numberOfSeats = VehicleDefaults.DefaultNumberOfSeats) : 
    Vehicle(plate, VehicleTypes.SUV, manufacturer, model, year, startingBid)
{
    [Range(2, int.MaxValue, ErrorMessage = "Number of seats must be a positive value greater than 1.")]
    public int NumberOfSeats { get; set; } = numberOfSeats;
    static public string PropertyNameNS => "NumberOfSeats";
}