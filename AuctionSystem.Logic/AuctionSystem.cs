using System.Collections.Concurrent;
using AuctionSystem.Logic.Factories;
using AuctionSystem.Logic.Models.Vehicles;

namespace AuctionSystem.Logic.Models;

// Singleton class to hold the state of the auction system
public class AuctionSystem
{
    
    private static AuctionSystem? _instance;
    private ConcurrentDictionary<string, Vehicle> Inventory { get; set; } = new ConcurrentDictionary<string, Vehicle>();
    private IVehicleFactory VehicleFactory { get; set; }

    /// Active auctions
    private ConcurrentDictionary<Auction, string> Auctions { get; set; } = new ConcurrentDictionary<Auction, string>();
    private ConcurrentDictionary<string, Guid?> VehicleAuctions { get; set; } = new ConcurrentDictionary<string, Guid?>();


    public static AuctionSystem Instance(IVehicleFactory vehicleFactory)
    {
        return _instance ??= new AuctionSystem(vehicleFactory);
    }
    
    // Allow tests to reset singleton state
    public void ResetState()
    {
        Inventory.Clear();
        Auctions.Clear();
        VehicleAuctions.Clear();
    }
    
    public AuctionSystem(IVehicleFactory vehicleFactory)
    {
        VehicleFactory = vehicleFactory;
    }

    public bool CreateVehicle(string plate, string type, string manufacturer, string model, int year, int startingBid, Dictionary<string, object>? extras = null)
    {
        try
        {
            var vehicle = VehicleFactory.CreateVehicle(plate, type, manufacturer, model, year, startingBid, extras ?? new Dictionary<string, object>());
            if (Inventory.TryAdd(plate, vehicle))
            {
                VehicleAuctions[plate] = null;
                return true;
            }
            
            throw new ArgumentException($"A vehicle with plate {plate} already exists in the inventory.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating vehicle: {ex.Message}");
            return false;
        }
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
        try
        {  
            if (!Inventory.TryGetValue(plate, out var vehicle))
            {           
            throw new ArgumentException($"Vehicle with plate {plate} not found in the system.");
            }

            if(VehicleAuctions.TryGetValue(plate, out Guid? existingAuctionId) && existingAuctionId.HasValue)
            {
                throw new InvalidOperationException($"Vehicle with plate {plate} is already assigned to auction with ID {existingAuctionId}.");
            }
        
            var auction = new Auction(plate, vehicle.StartingBid, out Guid auctionId);
            if (VehicleAuctions.TryUpdate(plate, auctionId, null))
            {
                Auctions.TryAdd(auction, plate);
                Console.WriteLine($"Auction with ID {auctionId} started for vehicle with plate {plate}.");
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting auction: {ex.Message}");
        }
        return false; 
    }

    public bool PlaceBid(string plate, string user, int bidAmount)
    {   
        try
        {
            VerifyVehicleAuction(plate, out var auction);
            if(auction != null)
            {
                return auction.BidOnVehicle(user, bidAmount);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error placing bid: {ex.Message}");
        }
        return false;
    }

    public int EndAuction(string plate, bool isSold)
    {
        try
        {
            VerifyVehicleAuction(plate, out var auction);
            if(auction != null)
            {
                if (Auctions.TryRemove(auction, out _))
                {
                    if (!isSold)
                    {
                        Console.WriteLine($"Auction with ID {auction.Id} ended for vehicle with plate {plate}. Vehicle not sold.");
                        VehicleAuctions[plate] = null;
                        return 0;
                    }
                    else
                    {
                        Inventory.TryRemove(plate, out _);
                        VehicleAuctions.TryRemove(plate, out _);
                        Console.WriteLine($"Auction with ID {auction.Id} ended for vehicle with plate {plate}. Vehicle sold.");
                        int finalBid = auction.GetCurrentBid();
                        Auctions.Remove(auction, out _);
                        return finalBid;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error ending auction: {ex.Message}");
        }
        return -1;

    }

    public List<Auction> GetActiveAuctions()
    {
        return Auctions.Keys.ToList();
    }

    public void PrintAuctionLog(string plate)
    {
        try
        {
            VerifyVehicleAuction(plate, out var auction);
            if(auction != null)
            {
                var log = auction.GetBiddingLog();
                Console.WriteLine($"Bidding log for vehicle with plate {plate}:");
                foreach (var entry in log)
                {
                    Console.WriteLine($"{entry.Timestamp}: {entry.User} bid ${entry.BidAmount}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error printing auction log: {ex.Message}");
        }
    }

    private void VerifyVehicleAuction(string plate, out Auction? auction)
    {
        auction = null;
        if (!VehicleAuctions.TryGetValue(plate, out Guid? auctionId) || !auctionId.HasValue)
        {
            throw new ArgumentException($"No active auction found for vehicle with plate {plate}.");
        }

        auction = Auctions.Keys.FirstOrDefault(a => a.Id == auctionId.Value);
        if (auction == null)
        {
            throw new InvalidOperationException($"Auction with ID {auctionId} not found for vehicle with plate {plate}.");
        }
        return;
    }

    
}