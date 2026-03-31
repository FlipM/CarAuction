using AuctionSystem.Logic.Models.Vehicles;

namespace AuctionSystem.Logic.Factories;

public class VehicleFactory : IVehicleFactory
{
  
    public Vehicle CreateVehicle(string plate, string type, string manufacturer, string model, int year, int startingBid, Dictionary<string, object> extras)
    {
        if (string.IsNullOrWhiteSpace(plate) || plate.Length != 8 || plate.Any(c => !char.IsLetterOrDigit(c)))
        {
            throw new ArgumentException("Plate must be exactly 8 characters long, with only letters and digits.", nameof(plate));
        }

        return type.ToUpper() switch
        {
            VehicleTypes.Hatchback => CreateHatchback(plate, manufacturer, model, year, startingBid, extras),
            VehicleTypes.Sedan => CreateSedan(plate, manufacturer, model, year, startingBid, extras),
            VehicleTypes.SUV => CreateSUV(plate, manufacturer, model, year, startingBid, extras),
            VehicleTypes.Truck => CreateTruck(plate, manufacturer, model, year, startingBid, extras),
            _ => throw new ArgumentException($"Vehicle type '{type}' is not recognized.")
        };
        
    }

    private Hatchback CreateHatchback(string plate, string manufacturer, string model, int year, int startingBid, Dictionary<string, object> extras)
    {
        int doors = GetIntFromExtras(extras, "NumberOfDoors", VehicleDefaults.DefaultHatchbackDoors);
        return new Hatchback(plate, manufacturer, model, year, startingBid, doors);
    }

    private Sedan CreateSedan(string plate, string manufacturer, string model, int year, int startingBid, Dictionary<string, object> extras)
    {
        int doors = GetIntFromExtras(extras, "NumberOfDoors", VehicleDefaults.DefaultSedanDoors);
        return new Sedan(plate, manufacturer, model, year, startingBid, doors);
    }

    private SUV CreateSUV(string plate, string manufacturer, string model, int year, int startingBid, Dictionary<string, object> extras)
    {
        int seats = GetIntFromExtras(extras, "NumberOfSeats", VehicleDefaults.DefaultNumberOfSeats);
        return new SUV(plate, manufacturer, model, year, startingBid, seats);
    }

    private Truck CreateTruck(string plate, string manufacturer, string model, int year, int startingBid, Dictionary<string, object> extras)
    {
        int loadCapacity = GetIntFromExtras(extras, "LoadCapacity", VehicleDefaults.DefaultLoadCapacity);
        return new Truck(plate, manufacturer, model, year, startingBid, loadCapacity);
    }

    private int GetIntFromExtras(Dictionary<string, object> extras, string key, int defaultValue)
    {
        return extras.TryGetValue(key, out var value) && value is int intValue 
                ? intValue 
                : defaultValue;
    }
}