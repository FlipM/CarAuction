using System.ComponentModel.DataAnnotations;
using AuctionSystem.Logic.Models.Vehicles;

namespace AuctionSystem.Logic.Factories;

public class VehicleFactory : IVehicleFactory
{
  
    public Vehicle CreateVehicle(string plate, string type, string manufacturer, string model, int year, int startingBid, Dictionary<string, object> extras)
    {
        Vehicle vehicle = type.ToUpper() switch
        {
            VehicleTypes.Hatchback => CreateHatchback(plate, manufacturer, model, year, startingBid, extras),
            VehicleTypes.Sedan => CreateSedan(plate, manufacturer, model, year, startingBid, extras),
            VehicleTypes.SUV => CreateSUV(plate, manufacturer, model, year, startingBid, extras),
            VehicleTypes.Truck => CreateTruck(plate, manufacturer, model, year, startingBid, extras),
            _ => throw new ArgumentException($"Vehicle type '{type}' is not recognized.")
        };

        var context = new ValidationContext(vehicle);
        var results = new List<ValidationResult>();

        if (!Validator.TryValidateObject(vehicle, context, results, true))
        {
            var errors = string.Join(" ", results.Select(r => r.ErrorMessage));
            throw new ArgumentException($"Vehicle validation failed: {errors}");
        }

        return vehicle;
    }

    private Hatchback CreateHatchback(string plate, string manufacturer, string model, int year, int startingBid, Dictionary<string, object> extras)
    {
        int doors = GetIntFromExtras(extras, Hatchback.PropertyNameND, VehicleDefaults.DefaultHatchbackDoors);
        return new Hatchback(plate, manufacturer, model, year, startingBid, doors);
    }

    private Sedan CreateSedan(string plate, string manufacturer, string model, int year, int startingBid, Dictionary<string, object> extras)
    {
        int doors = GetIntFromExtras(extras, Sedan.PropertyNameND, VehicleDefaults.DefaultSedanDoors);
        return new Sedan(plate, manufacturer, model, year, startingBid, doors);
    }

    private SUV CreateSUV(string plate, string manufacturer, string model, int year, int startingBid, Dictionary<string, object> extras)
    {
        int seats = GetIntFromExtras(extras, SUV.PropertyNameNS, VehicleDefaults.DefaultNumberOfSeats);
        return new SUV(plate, manufacturer, model, year, startingBid, seats);
    }

    private Truck CreateTruck(string plate, string manufacturer, string model, int year, int startingBid, Dictionary<string, object> extras)
    {
        int loadCapacity = GetIntFromExtras(extras, Truck.PropertyNameLC, VehicleDefaults.DefaultLoadCapacity);
        return new Truck(plate, manufacturer, model, year, startingBid, loadCapacity);
    }

    private int GetIntFromExtras(Dictionary<string, object> extras, string key, int defaultValue)
    {
        return extras.TryGetValue(key, out var value) && value is int intValue 
                ? intValue 
                : defaultValue;
    }
}