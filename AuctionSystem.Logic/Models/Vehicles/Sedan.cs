namespace AuctionSystem.Logic.Models.Vehicles;
using System.ComponentModel.DataAnnotations;

public class Sedan(string plate, string manufacturer, string model, int year, int startingBid, int numberOfDoors = VehicleDefaults.DefaultSedanDoors) : 
    Vehicle(plate, VehicleTypes.Sedan, manufacturer, model, year, startingBid)
{
    [Range(2, int.MaxValue, ErrorMessage = "Number of doors must be a positive value greater than 1.")]
    public int NumberOfDoors { get; set; } = numberOfDoors;
    static public string PropertyNameND => "NumberofDoors";
}