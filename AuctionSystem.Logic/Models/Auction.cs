using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;

namespace AuctionSystem.Logic;


public class Auction
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(8), MinLength(6)]
    public string VehiclePlate { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "Current bid must be a positive value.")]
    private int CurrentBid { get; set; } = 0;

    private readonly object BidLock = new();


    private ConcurrentQueue<(DateTime Timestamp, string User, int BidAmount)> BiddingLogQueue { get; set; } = new ConcurrentQueue<(DateTime Timestamp, string User, int BidAmount)>();

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

        lock (BidLock)
        {
            if (bidAmount <= CurrentBid)
            {
                Console.WriteLine($"Bid amount ${bidAmount} is not higher than current bid for vehicle with plate {VehiclePlate}.");
                return false;
            }
            else
            {
                BiddingLogQueue.Enqueue((DateTime.UtcNow, user, bidAmount));
                CurrentBid = bidAmount;
                Console.WriteLine($"Bid placed on vehicle with plate {VehiclePlate} for ${bidAmount}.");

                return true;
            }
        }
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
        var log = BiddingLogQueue.ToList();
        return log;
    }

}
