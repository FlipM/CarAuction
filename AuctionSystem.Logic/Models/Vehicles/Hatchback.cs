namespace AuctionSystem.Logic.Models.Vehicles;
using System.ComponentModel.DataAnnotations;

public class Hatchback(string plate, string manufacturer, string model, int year, int startingBid, int numberOfDoors = VehicleDefaults.DefaultHatchbackDoors) :
    Vehicle(plate, VehicleTypes.Hatchback, manufacturer, model, year, startingBid)
{
    [Range(2, int.MaxValue, ErrorMessage = "Number of doors must be a positive value greater than 1.")]
    public int NumberOfDoors { get; set; } = numberOfDoors;
    
}