using AuctionSystemModel = AuctionSystem.Logic.Models.AuctionSystem;
using AuctionSystem.Logic.Factories;

static class Program
{   
    static public int Main()
    {
        var auctionSystem = AuctionSystemModel.Instance(new VehicleFactory());


        string sedanPlate = "ABC12345";
        string hatchbackPlate = "DEF67890";
        string suvPlate = "GHI54321";
        var sedanOk = auctionSystem.CreateVehicle(sedanPlate, "SED", "Toyota", "Camry", 2020, 15000, new Dictionary<string, object> { { "numberOfDoors", 4 } });
        var hatchbackOk = auctionSystem.CreateVehicle(hatchbackPlate, "HCB", "Honda", "Civic", 2019, 12000, new Dictionary<string, object> { { "numberOfDoors", 5 } });
        var suvOk = auctionSystem.CreateVehicle(suvPlate, "SUV", "Ford", "Explorer", 2021, 25000, new Dictionary<string, object> { { "numberOfSeats", 7 } });

        if (!sedanOk || !hatchbackOk || !suvOk)
        {
            Console.WriteLine("Error creating vehicles. Please check the input data.");
            return -1;
        }

        Console.WriteLine("Vehicles added to inventory:");
        foreach (var vehicle in auctionSystem.Inventory.Values)
        {
            Console.WriteLine($"{vehicle.Type}: {vehicle.Manufacturer} {vehicle.Model} ({vehicle.Year}) - Starting Bid: ${vehicle.StartingBid}");
        }


        if(auctionSystem.StartAuction(sedanPlate))
        {
            Console.WriteLine("Created auction for sedan.");
        }
        else
        {
            Console.WriteLine("Failed to create auction for sedan.");
        }
        
        auctionSystem.GetActiveAuctions();
        Console.WriteLine("Active auctions:");
        foreach (var auction in auctionSystem.GetActiveAuctions())
        {
            Console.WriteLine($"Auction for vehicle with plate {auction.VehiclePlate} - Current Bid: ${auction.GetCurrentBid()}");
        }

        auctionSystem.PlaceBid(sedanPlate, "User1", 16000);
        auctionSystem.PlaceBid(sedanPlate, "User2", 17000);

        auctionSystem.PrintAuctionLog(sedanPlate);
        auctionSystem.PrintAuctionLog(suvPlate);

        auctionSystem.PlaceBid(sedanPlate, "User3", 26000);
        auctionSystem.PlaceBid(suvPlate, "User3", 26000);
        int sedanAuctionResult = auctionSystem.EndAuction(sedanPlate, true);
        int suvAuctionResult = auctionSystem.EndAuction(suvPlate, false);
        auctionSystem.PlaceBid(sedanPlate, "User4", 50000);

        var badVehicleOk = auctionSystem.CreateVehicle("INVAL ID", "SED", "Chevrolet", "Camaro", 2019, 10000);
        var badVehicle2Ok = auctionSystem.CreateVehicle("INVAL ID", "GED", "Chevrolet", "Camaro", 2020, 10000);
        var badVehicle3Ok = auctionSystem.CreateVehicle("INVAL ID", "SED", "Chevro!et", "Camaro", 2021, 10000);
        var badVehicle4Ok = auctionSystem.CreateVehicle("INVAL ID", "SED", "Chevrolet", "Cam4ro", 2022, 10000);
        var badVehicle5Ok = auctionSystem.CreateVehicle("INVAL ID", "SED", "Chevrolet", "Camaro", 1700, 10000);
        var badVehicle6Ok = auctionSystem.CreateVehicle("INVAL ID", "SED", "Chevrolet", "Camaro", 2023, -2);

        if(badVehicleOk || badVehicle2Ok || badVehicle3Ok || badVehicle4Ok || badVehicle5Ok || badVehicle6Ok)
        {
            Console.WriteLine("Error: Invalid vehicle was created successfully. Please check validation logic.");
        }
        else
        {
            Console.WriteLine("Invalid vehicles were correctly rejected by the system.");
        }

        var vehicles = auctionSystem.GetFilteredAuctionVehicles();

        foreach (var vehicle in vehicles)
        {
            Console.WriteLine($"Filtered Vehicle - {vehicle.Type}: {vehicle.Manufacturer} {vehicle.Model} ({vehicle.Year}) - Starting Bid: ${vehicle.StartingBid}");
        }
        

        var sedan2Ok = auctionSystem.CreateVehicle("JKL98765", "SED", "Chevrolet", "Camaro", 2020, 22000);
        var sedan3Ok = auctionSystem.CreateVehicle("MNO24680", "SED", "BMW", "3 Series", 2020, 40000);

        if(!sedan2Ok || !sedan3Ok)
        {
            Console.WriteLine("Error creating additional sedans. Please check the input data.");
        }
        else
        {
            Console.WriteLine("Additional sedans added to inventory successfully.");
        }



        return 0;
    }
}