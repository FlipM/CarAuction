using AuctionSystem.Logic.Models.Vehicles;

namespace AuctionSystem.Logic.Factories;

public class VehicleFactory : IVehicleFactory
{
  
    public Vehicle CreateVehicle(string type, string manufacturer, string model, int year, int startingBid, Dictionary<string, object> extras)
    {

        return type.ToUpper() switch
        {
            VehicleTypes.Hatchback => CreateHatchback(manufacturer, model, year, startingBid, extras),
            VehicleTypes.Sedan => CreateSedan(manufacturer, model, year, startingBid, extras),
            VehicleTypes.SUV => CreateSUV(manufacturer, model, year, startingBid, extras),
            VehicleTypes.Truck => CreateTruck(manufacturer, model, year, startingBid, extras),
            _ => throw new ArgumentException($"Vehicle type '{type}' is not recognized.")
        };
        
    }

    private Hatchback CreateHatchback(string manufacturer, string model, int year, int startingBid, Dictionary<string, object> extras)
    {
        int doors = GetIntFromExtras(extras, "NumberOfDoors", VehicleDefaults.DefaultHatchbackDoors);
        return new Hatchback(manufacturer, model, year, startingBid, doors);
    }

    private Sedan CreateSedan(string manufacturer, string model, int year, int startingBid, Dictionary<string, object> extras)
    {
        int doors = GetIntFromExtras(extras, "NumberOfDoors", VehicleDefaults.DefaultSedanDoors);
        return new Sedan(manufacturer, model, year, startingBid, doors);
    }

    private SUV CreateSUV(string manufacturer, string model, int year, int startingBid, Dictionary<string, object> extras)
    {
        int seats = GetIntFromExtras(extras, "NumberOfSeats", VehicleDefaults.DefaultNumberOfSeats);
        return new SUV(manufacturer, model, year, startingBid, seats);
    }

    private Truck CreateTruck(string manufacturer, string model, int year, int startingBid, Dictionary<string, object> extras)
    {
        int loadCapacity = GetIntFromExtras(extras, "LoadCapacity", VehicleDefaults.DefaultLoadCapacity);
        return new Truck(manufacturer, model, year, startingBid, loadCapacity);
    }

    private int GetIntFromExtras(Dictionary<string, object> extras, string key, int defaultValue)
    {
        return extras.TryGetValue(key, out var value) && value is int intValue 
                ? intValue 
                : defaultValue;
    }
}