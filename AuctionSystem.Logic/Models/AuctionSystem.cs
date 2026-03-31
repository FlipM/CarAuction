using System.Collections.Concurrent;
using AuctionSystem.Logic.Factories;
using AuctionSystem.Logic.Models.Vehicles;

namespace AuctionSystem.Logic.Models;

// Singleton class to hold the state of the auction system
public class AuctionSystem
{
    
    private static AuctionSystem? _instance;
    public ConcurrentDictionary<string, Vehicle> Inventory { get; set; } = new ConcurrentDictionary<string, Vehicle>();
    public IVehicleFactory VehicleFactory { get; set; }

    /// Active auctions
    private ConcurrentDictionary<Auction, string> Auctions { get; set; } = new ConcurrentDictionary<Auction, string>();
    private ConcurrentDictionary<string, Guid?> VehicleAuctions { get; set; } = new ConcurrentDictionary<string, Guid?>();


    public static AuctionSystem Instance(IVehicleFactory vehicleFactory)
    {
        return _instance ??= new AuctionSystem(vehicleFactory);
    }
    
    public AuctionSystem(IVehicleFactory vehicleFactory)
    {
        VehicleFactory = vehicleFactory;
    }

    bool CreateVehicle(string plate, string type, string manufacturer, string model, int year, int startingBid, Dictionary<string, object> extras)
    {
        try
        {
            if (Inventory.TryGetValue(plate, out _))
            {
                Console.WriteLine($"Vehicle with plate {plate} already exists in the system.");
                return false;
            }

            var vehicle = VehicleFactory.CreateVehicle(plate, type, manufacturer, model, year, startingBid, extras);
            Inventory[plate] = vehicle;
            VehicleAuctions[plate] = null;
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating vehicle: {ex.Message}");
            return false;
        }
    }

    public IEnumerable<Vehicle> GetFilteredAuctionVehicles(string? type, string? model, string? manufacturer, int? year)
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

    bool StartAuction(string plate)
    {
        if (!Inventory.TryGetValue(plate, out var vehicle))
        {
            Console.WriteLine($"Vehicle with plate {plate} not found in the system.");
            return false;
        }

        if(VehicleAuctions.TryGetValue(plate, out Guid? existingAuctionId) && existingAuctionId.HasValue)
        {
            Console.WriteLine($"Vehicle with plate {plate} is already assigned to auction with ID {existingAuctionId}.");
            return false;
        }

        var auction = new Auction(plate, vehicle.StartingBid, out Guid auctionId);


        if (Auctions.TryAdd(auction, plate) && VehicleAuctions.TryUpdate(plate, auctionId, null))
        {
            Console.WriteLine($"Auction with ID {auctionId} started for vehicle with plate {plate}.");
            return true;
        }
        
        return false;
    }

    bool PlaceBid(string plate, string user, int bidAmount)
    {
        if (!VehicleAuctions.TryGetValue(plate, out Guid? auctionId) || !auctionId.HasValue)
        {
            Console.WriteLine($"No active auction found for vehicle with plate {plate}.");
            return false;
        }

        var auction = Auctions.Keys.FirstOrDefault(a => a.Id == auctionId.Value);
        if (auction == null)
        {
            Console.WriteLine($"Auction missmatch: No auction found with ID {auctionId} for vehicle with plate {plate}.");
            return false;
        }

        return auction.BidOnVehicle(user, bidAmount);
    }

    int EndAuction(string plate, bool isSold)
    {
        if (!VehicleAuctions.TryGetValue(plate, out Guid? auctionId) || !auctionId.HasValue)
        {
            Console.WriteLine($"No active auction found for vehicle with plate {plate}.");
            return -1;
        }

        var auction = Auctions.Keys.FirstOrDefault(a => a.Id == auctionId.Value);
        if (auction == null)
        {
            Console.WriteLine($"Auction missmatch: No auction found with ID {auctionId} for vehicle with plate {plate}.");
            return -1;
        }

        if (Auctions.TryRemove(auction, out _))
        {
            if (!isSold)
            {
                Console.WriteLine($"Auction with ID {auctionId} ended for vehicle with plate {plate}. Vehicle not sold.");
                VehicleAuctions[plate] = null;
                return 0;
            }
            else
            {
                Inventory.TryRemove(plate, out _);
                VehicleAuctions.TryRemove(plate, out _);
                Console.WriteLine($"Auction with ID {auctionId} ended for vehicle with plate {plate}. Vehicle sold.");
                int finalBid = auction.GetCurrentBid();
                Auctions.Remove(auction, out _);
                return finalBid;
            }
        }
        
        return -1;
    }

    
}