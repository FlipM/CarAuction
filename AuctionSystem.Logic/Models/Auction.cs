using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;

namespace AuctionSystem.Logic.Models;


public class Auction
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(8), MinLength(6)]
    public string VehiclePlate { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "Current bid must be a positive value.")]
    private int CurrentBid { get; set; } = 0;

    private readonly object BidLock = new();


    private ConcurrentDictionary<DateTime, (string User, int BidAmount)> BiddingLogDict { get; set; } = new ConcurrentDictionary<DateTime, (string User, int BidAmount)>();

    public Auction(string plate, int initialBid, out Guid auctionId)
    {
        VehiclePlate = plate;
        CurrentBid = initialBid;
        auctionId = Id;
    }   

    public bool BidOnVehicle(string user, int bidAmount)
    {

        if (bidAmount < 0)
        {
            Console.WriteLine($"Bid amount cannot be negative for vehicle with plate {VehiclePlate}.");
            return false;
        }

        if (bidAmount > Int32.MaxValue)
        {
            Console.WriteLine($"Bid amount is out of bounds. Talk to our support team for more information about bidding limits.");
            return false;
        }

        lock (BidLock)
        {
            if (bidAmount <= CurrentBid)
            {
                Console.WriteLine($"Bid amount ${bidAmount} is not higher than current bid for vehicle with plate {VehiclePlate}.");
                return false;
            }
            
            if (bidAmount > CurrentBid)
            {
                BiddingLogDict[DateTime.Now] = (user, bidAmount);
                CurrentBid = bidAmount;
                Console.WriteLine($"Bid placed on vehicle with plate {VehiclePlate} for ${bidAmount}.");

                return true;
            }
        }

        Console.WriteLine($"Bid amount ${bidAmount} is not higher than current bid for vehicle with plate {VehiclePlate}.");
        return false;
    }

    public int GetCurrentBid()
    {
        lock (BidLock)
        {
            return CurrentBid;
        }
    }

    public List<(DateTime Timestamp, string User, int BidAmount)> GetBiddingLog()
    {
        var log = BiddingLogDict.Select(entry => (entry.Key, entry.Value.User, entry.Value.BidAmount)).ToList();
        return log;
    }

}
