using System.Collections.Concurrent;
using AuctionSystem.Logic.Factories;
using AuctionSystem.Logic.Models.Vehicles;

namespace AuctionSystem.Logic.Models;

// Singleton class to hold the state of the auction system
public class AuctionSystem
{
    private ConcurrentDictionary<string, Vehicle> Inventory { get; set; } = new ConcurrentDictionary<string, Vehicle>();
    private IVehicleFactory VehicleFactory { get; set; }

    /// Active auctions
    private ConcurrentDictionary<Guid, Auction> Auctions { get; set; } = new ConcurrentDictionary<Guid, Auction>();
    private ConcurrentDictionary<string, Guid> VehicleAuctions { get; set; } = new ConcurrentDictionary<string, Guid>();
    
    public AuctionSystem(IVehicleFactory vehicleFactory)
    {
        VehicleFactory = vehicleFactory;
    }

    public bool CreateVehicle(string plate, string type, string manufacturer, string model, int year, int startingBid, Dictionary<string, object>? extras = null)
    {
        var vehicle = VehicleFactory.CreateVehicle(plate, type, manufacturer, model, year, startingBid, extras ?? new Dictionary<string, object>());
        if (Inventory.TryAdd(plate, vehicle))
        {
            return true;
        }

        throw new ArgumentException($"A vehicle with plate {plate} already exists in the inventory.");
    }

    public IEnumerable<Vehicle> GetFilteredAuctionVehicles(string? type = null, string? model = null, string? manufacturer = null, int? year = null)
    {
        IEnumerable<Vehicle> filtered = Inventory.Values;

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

    public bool StartAuction(string plate)
    {
        if (!Inventory.TryGetValue(plate, out var vehicle))
        {
            throw new ArgumentException($"Vehicle with plate {plate} not found in the system.");
        }

        if (VehicleAuctions.ContainsKey(plate))
        {
            throw new InvalidOperationException($"Vehicle with plate {plate} is already assigned to an active auction.");
        }

        var auction = new Auction(plate, vehicle.StartingBid, out Guid auctionId);
        if (!VehicleAuctions.TryAdd(plate, auctionId))
        {
            throw new InvalidOperationException($"Failed to register auction for vehicle with plate {plate}.");
        }

        if (Auctions.TryAdd(auctionId, auction))
        {
            return true;
        }

        VehicleAuctions.TryRemove(plate, out _);
        throw new InvalidOperationException($"Failed to start auction for vehicle with plate {plate}.");
    }

    public bool PlaceBid(string plate, string user, int bidAmount)
    {   
        VerifyVehicleAuction(plate, out var auction);

        return auction.BidOnVehicle(user, bidAmount);
    }

    public int EndAuction(string plate, bool isSold)
    {
        VerifyVehicleAuction(plate, out var auction);

        if (!VehicleAuctions.TryRemove(plate, out var auctionId))
        {
            throw new InvalidOperationException($"No active auction association found for vehicle with plate {plate}.");
        }

        if (!Auctions.TryRemove(auctionId, out _))
        {
            throw new InvalidOperationException($"Auction with ID {auctionId} was not found among active auctions.");
        }

        if (!isSold)
        {
            return 0;
        }

        Inventory.TryRemove(plate, out _);
        return auction.GetCurrentBid();
    }

    public List<Auction> GetActiveAuctions()
    {
        return Auctions.Values.ToList();
    }

    public void PrintAuctionLog(string plate)
    {
        VerifyVehicleAuction(plate, out var auction);

        var log = auction.GetBiddingLog();
        Console.WriteLine($"Bidding log for vehicle with plate {plate}:");
        foreach (var entry in log)
        {
            Console.WriteLine($"{entry.Timestamp}: {entry.User} bid ${entry.BidAmount}");
        }
    }

    private void VerifyVehicleAuction(string plate, out Auction auction)
    {
        auction = null!;
        if (!VehicleAuctions.TryGetValue(plate, out var auctionId))
        {
            throw new ArgumentException($"No active auction found for vehicle with plate {plate}.");
        }

        if (!Auctions.TryGetValue(auctionId, out auction!))
        {
            throw new InvalidOperationException($"Auction with ID {auctionId} not found for vehicle with plate {plate}.");
        }
    }

    
}