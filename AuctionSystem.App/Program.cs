using AuctionSystemModel = AuctionSystem.Logic.Models.AuctionSystem;
using AuctionSystem.Logic.Factories;

static class Program
{   
    static public int Main()
    {
        var auctionSystem = AuctionSystemModel.Instance(new VehicleFactory());

        var sedan = auctionSystem.VehicleFactory.CreateVehicle("ABC12345", "SED", "Toyota", "Camry", 2020, 15000, new Dictionary<string, object> { { "numberOfDoors", 4 } });
        var hatchback = auctionSystem.VehicleFactory.CreateVehicle("DEF67890", "HCB", "Honda", "Civic", 2019, 12000, new Dictionary<string, object> { { "numberOfDoors", 5 } });
        var suv = auctionSystem.VehicleFactory.CreateVehicle("GHI54321", "SUV", "Ford", "Explorer", 2021, 25000, new Dictionary<string, object> { { "numberOfSeats", 7 } });

        auctionSystem.Inventory[sedan.Plate] = sedan;
        auctionSystem.Inventory[hatchback.Plate] = hatchback;
        auctionSystem.Inventory[suv.Plate] = suv;

        Console.WriteLine("Vehicles added to inventory:");
        foreach (var vehicle in auctionSystem.Inventory.Values)
        {
            Console.WriteLine($"{vehicle.Type}: {vehicle.Manufacturer} {vehicle.Model} ({vehicle.Year}) - Starting Bid: ${vehicle.StartingBid}");
        }

        return 0;
    }
}