namespace AuctionSystem.Logic.Models.Vehicles;
using System.ComponentModel.DataAnnotations;

public class Truck(string plate, string manufacturer, string model, int year, int startingBid, int loadCapacity = VehicleDefaults.DefaultLoadCapacity) : 
    Vehicle(plate, VehicleTypes.Truck, manufacturer, model, year, startingBid)
{
    [Range(1, int.MaxValue, ErrorMessage = "Load capacity must be a positive value.")]
    public int LoadCapacity { get; set; } = loadCapacity;
    static public string PropertyNameLC => "LoadCapacity";
}