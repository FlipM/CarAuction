using AuctionSystem.Logic.Models.Vehicles;
using AuctionSystem.Logic.Factories;
using System.Collections.Concurrent;

namespace AuctionSystem.Logic.Models;


public class Auction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;

    private ConcurrentDictionary<Vehicle, int> _inventory { get; set; } = new ConcurrentDictionary<Vehicle, int>();
    private readonly IVehicleFactory _vehicleFactory;

    public Auction(IVehicleFactory vehicleFactory, out Guid auctionId)
    {
        _vehicleFactory = vehicleFactory;
        auctionId = Id;
    }   

    public void AddVehicle(string type, string manufacturer, string model, int year, int startingBid, Dictionary<string, object>? additionalParams = null)
    {
        Vehicle vehicle = _vehicleFactory.CreateVehicle(type, manufacturer, model, year, startingBid, additionalParams ?? new Dictionary<string, object>());
         _inventory[vehicle] = startingBid;
    }

    public IEnumerable<Vehicle> GetFilteredAuctionVehicles(string? type, string? model, string? manufacturer, int? year)
    {
        IEnumerable<Vehicle> filtered = _inventory.Keys;

        if (!string.IsNullOrEmpty(type))
            filtered = filtered.Where(v => v.Type == type);

        if (!string.IsNullOrEmpty(model))
            filtered = filtered.Where(v => v.Model == model);

        if (!string.IsNullOrEmpty(manufacturer))
            filtered = filtered.Where(v => v.Manufacturer == manufacturer);

        if (year.HasValue)
            filtered = filtered.Where(v => v.Year == year.Value);

        return filtered;
    }

    public bool BidOnVehicle(Guid vehicleId, int bidAmount)
    {
        var vehicle = _inventory.Keys.FirstOrDefault(v => v.Id == vehicleId);
        if (vehicle == null)
        {
            Console.WriteLine($"Vehicle with ID {vehicleId} not found in auction, can't place bid.");
            return false;
        }

        if (bidAmount > _inventory[vehicle])
        {
            _inventory[vehicle] = bidAmount;
            Console.WriteLine($"Bid placed on vehicle with ID {vehicleId} for ${bidAmount}.");

            return true;
        }

        Console.WriteLine($"Bid amount ${bidAmount} is not higher than current bid for vehicle with ID {vehicleId}.");
        return false;
    }

}