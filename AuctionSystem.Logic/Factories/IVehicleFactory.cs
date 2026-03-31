using AuctionSystem.Logic.Models.Vehicles;

namespace AuctionSystem.Logic.Factories;

public interface IVehicleFactory
{
    public Vehicle CreateVehicle(string type, string manufacturer, string model, int year, int startingBid, Dictionary<string, object> extras);
}