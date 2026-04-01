using AuctionSystem.Logic.Factories;
using AuctionSystem.Logic.Models.Vehicles;
using AuctionSystemModel = AuctionSystem.Logic.Models.AuctionSystem;

namespace AuctionSystem.Tests;

public class UnitTest
{

    [Fact]
    public void VehicleCreationTests()
    {
        VehicleFactory vf = new VehicleFactory();
        var sedan = vf.CreateVehicle("ABC12345", VehicleTypes.Sedan, "Toyota", "Camry", 2020, 15000, new Dictionary<string, object> { { "nd", -1 } });
        var hatchback = vf.CreateVehicle("DEF67890", VehicleTypes.Hatchback, "Honda", "Civic", 2019, 12000, new Dictionary<string, object> { { "nd", 58 } });
        var suv = vf.CreateVehicle("GHI54321", VehicleTypes.SUV, "Ford", "Explorer", 2021, 25000, new Dictionary<string, object> { { "ns", 7 } });
        var truck = vf.CreateVehicle("JKL98765", VehicleTypes.Truck, "Chevrolet", "Silverado", 2022, 30000, new Dictionary<string, object> { { "LoadCapacity", 2050 } });

        Assert.NotNull(sedan);
        Assert.NotNull(hatchback);
        Assert.NotNull(suv);
        Assert.NotNull(truck);

        Assert.IsType<Sedan>(sedan);
        Assert.IsType<Hatchback>(hatchback);
        Assert.IsType<SUV>(suv);
        Assert.IsType<Truck>(truck);

        // Default values due to invalid input, except on truck
        Assert.Equal(4, ((Sedan)sedan).NumberOfDoors);
        Assert.Equal(4, ((Hatchback)hatchback).NumberOfDoors);
        Assert.Equal(5, ((SUV)suv).NumberOfSeats);
        Assert.Equal(2050, ((Truck)truck).LoadCapacity);
    }

    [Fact]
    public void InvalidVehicleCreationTests()
    {
        VehicleFactory vf = new VehicleFactory();
        var emptydict = new Dictionary<string, object>();
        Assert.Throws<ArgumentException>(() => vf.CreateVehicle("INVAL ID", VehicleTypes.Sedan, "Chevrolet", "Camaro", 2019, 10000, emptydict));
        Assert.Throws<ArgumentException>(() => vf.CreateVehicle("INVAL ID", "GED", "Chevrolet", "Camaro", 2020, 10000, emptydict));
        Assert.Throws<ArgumentException>(() => vf.CreateVehicle("INVAL ID", VehicleTypes.Truck, "Chevro!et", "Camaro", 2021, 10000, emptydict));
        Assert.Throws<ArgumentException>(() => vf.CreateVehicle("INVAL ID", VehicleTypes.Hatchback, "Chevrolet", "Cam4ro", 2022, 10000, emptydict));
        Assert.Throws<ArgumentException>(() => vf.CreateVehicle("INVAL ID", VehicleTypes.SUV, "Chevrolet", "Camaro", 1700, 10000, emptydict));
        Assert.Throws<ArgumentException>(() => vf.CreateVehicle("INVAL ID", VehicleTypes.SUV, "Chevrolet", "Camaro", 2023, -2, emptydict));
        Assert.Throws<ArgumentException>(() => vf.CreateVehicle("VALID123", VehicleTypes.Sedan, "Chevrolet", "Camaro", 2024, 20000, new Dictionary<string, object> { { Sedan.PropertyNameND, -1 } }));
    }

    [Fact]

    public void AuctionSystemSingleAuctionWorkflowTest()
    {
        var auctionSystem = AuctionSystemModel.Instance(new VehicleFactory());
        auctionSystem.ResetState();

        string sedanPlate = "ABC12345";
        var sedanOk = auctionSystem.CreateVehicle(sedanPlate, VehicleTypes.Sedan, "Toyota", "Camry", 2020, 15000, new Dictionary<string, object> { { Sedan.PropertyNameND, 4 } });
        Assert.True(sedanOk, "Failed to create sedan vehicle.");
        Assert.True(auctionSystem.GetFilteredAuctionVehicles().Any(v => v.Plate == sedanPlate), "Sedan not found in inventory after creation.");

        bool auctionStarted = auctionSystem.StartAuction(sedanPlate);
        Assert.True(auctionStarted, "Failed to start auction for sedan.");
        Assert.True(auctionSystem.GetActiveAuctions().Any(a => a.VehiclePlate == sedanPlate), "Sedan auction not found in active auctions after starting.");

        Assert.True(auctionSystem.PlaceBid(sedanPlate, "User1", 16000) && auctionSystem.PlaceBid(sedanPlate, "User2", 17000), "Failed to place bids on sedan auction.");
        Assert.False(auctionSystem.PlaceBid(sedanPlate, "User3", 16000), "Was able to place bid that is not higher than current bid.");
        Assert.False(auctionSystem.PlaceBid(sedanPlate, "User3", -1), "Was able to place negative bid.");
        
        var activeAuctions = auctionSystem.GetActiveAuctions();
        Assert.Single(activeAuctions, a => a.VehiclePlate == sedanPlate && a.GetCurrentBid() == 17000);


        int auctionResult = auctionSystem.EndAuction(sedanPlate, true);
        Assert.Equal(17000, auctionResult);
        Assert.False(auctionSystem.GetFilteredAuctionVehicles().Any(v => v.Plate == sedanPlate), "Sedan should not be in inventory after being sold.");
        Assert.False(auctionSystem.GetActiveAuctions().Any(a => a.VehiclePlate == sedanPlate), "Sedan auction should not be active after auction ended.");

        // Attempt to place bid after auction ended
        bool bidAfterEnd = auctionSystem.PlaceBid(sedanPlate, "User3", 18000);
        Assert.False(bidAfterEnd, "Was able to place bid after auction ended.");

        // Attempt to end auction again
        int secondEndAttempt = auctionSystem.EndAuction(sedanPlate, true);
        Assert.Equal(-1, secondEndAttempt);

        // Attempt to start auction for non-existent vehicle
        bool startNonExistent = auctionSystem.StartAuction(sedanPlate);
        Assert.False(startNonExistent, "Was able to start auction for sold vehicle.");
    }

    [Fact]
    public void AuctionMultipleAuctionsTest()
    {
        var auctionSystem = AuctionSystemModel.Instance(new VehicleFactory());
        auctionSystem.ResetState();
        string sedanPlate = "ABC12345";
        string suvPlate = "GHI54321";
        string hatchbackPlate = "DEF67890";

        auctionSystem.CreateVehicle(sedanPlate, VehicleTypes.Sedan, "Toyota", "Camry", 2020, 15000, new Dictionary<string, object> { { Sedan.PropertyNameND, 4 } });
        auctionSystem.CreateVehicle(suvPlate, VehicleTypes.SUV, "Ford", "Explorer", 2021, 25000, new Dictionary<string, object> { { SUV.PropertyNameNS, 7 } });
        auctionSystem.CreateVehicle(hatchbackPlate, VehicleTypes.Hatchback, "Honda", "Civic", 2021, 18000, new Dictionary<string, object> { { Hatchback.PropertyNameND, 4 } });
        Assert.False(auctionSystem.CreateVehicle(sedanPlate, VehicleTypes.Sedan, "Toyota", "Corolla", 2021, 17000, new Dictionary<string, object> { { Sedan.PropertyNameND, 4 } }));

        Assert.True(auctionSystem.StartAuction(sedanPlate) && auctionSystem.StartAuction(suvPlate), "Failed to start auctions for sedan and SUV.");
        Assert.True(auctionSystem.GetActiveAuctions().Count() == 2, "Active auctions count should be 2 after starting sedan and SUV auctions.");
        Assert.False(auctionSystem.StartAuction(sedanPlate), "Was able to start auction for sedan again while it's already active.");

        Assert.True(auctionSystem.PlaceBid(sedanPlate, "User1", 16000) && auctionSystem.PlaceBid(suvPlate, "User2", 26000), "Failed to place bids on sedan and SUV auctions.");
        Assert.False(auctionSystem.PlaceBid(hatchbackPlate, "User3", 19000), "Hatchback is not auctioned.");   

        var activeAuctions = auctionSystem.GetActiveAuctions();
        Assert.Single(activeAuctions, a => a.VehiclePlate == sedanPlate && a.GetCurrentBid() == 16000);
        Assert.Single(activeAuctions, a => a.VehiclePlate == suvPlate && a.GetCurrentBid() == 26000);

        int sedanAuctionResult = auctionSystem.EndAuction(sedanPlate, true);
        int suvAuctionResult = auctionSystem.EndAuction(suvPlate, false);
        Assert.Equal(16000, sedanAuctionResult);
        Assert.Equal(0, suvAuctionResult);

        Assert.True(auctionSystem.GetFilteredAuctionVehicles().Any(v => v.Plate == suvPlate), "SUV should still be in inventory after failed auction.");
        Assert.False(auctionSystem.GetFilteredAuctionVehicles().Any(v => v.Plate == sedanPlate), "Sedan should not be in inventory after successful auction.");
        Assert.False(auctionSystem.PlaceBid(suvPlate, "User3", 27000), "Was able to place bid on SUV after auction ended, with the vehicle on the inventory.");

    }

    [Fact]
    public void ConcurrencyTests()
    {
        var auctionSystem = AuctionSystemModel.Instance(new VehicleFactory());
        auctionSystem.ResetState();
        string sedanPlate = "ABC12345";

        Parallel.For(0, 5, i =>
        {
            auctionSystem.CreateVehicle(sedanPlate, VehicleTypes.Sedan, "Toyota", "Camry", 2020, 15000, new Dictionary<string, object> { { Sedan.PropertyNameND, 4 } });
        });
        Assert.True(auctionSystem.GetFilteredAuctionVehicles().Count(v => v.Plate == sedanPlate) == 1, "There should only be one vehicle with the same plate in inventory.");

        Parallel.For(0, 5, i =>
        {
            auctionSystem.StartAuction(sedanPlate);
        });
        Assert.True(auctionSystem.GetActiveAuctions().Count() == 1, "There should only be one active auction.");

        int successfulBids = 0;
        int totalBids = 100;
        int highestBid = 0;
        Parallel.For(0, totalBids, i =>
        {
            if (auctionSystem.PlaceBid(sedanPlate, $"User{i}", 15000 + (i + 1) * 100))
            {
                Interlocked.Increment(ref successfulBids);
                Interlocked.Exchange(ref highestBid, 15000 + (i + 1) * 100);

            }
        });

        Assert.True(successfulBids > 0, "At least one bid should be successful.");
        Assert.True(15000 + totalBids * successfulBids < highestBid, "Highest bid should be greater than the initial bid plus increments of successful bids.");
        var activeAuctions = auctionSystem.GetActiveAuctions();
        Assert.Single(activeAuctions, a => a.VehiclePlate == sedanPlate && a.GetCurrentBid() == 15000 + totalBids * 100);
    }

    [Fact]
    public void StressTests()
    {
        var auctionSystem = AuctionSystemModel.Instance(new VehicleFactory());
        auctionSystem.ResetState();
        
        for (int i = 0; i < 10000; i++)
        {
            string plate = $"PL{i:D5}";

            Assert.True(auctionSystem.CreateVehicle(plate, VehicleTypes.Sedan, "Toyota", "Camry", 2020, 15000, new Dictionary<string, object> { { Sedan.PropertyNameND, 4 } }), $"Failed to create vehicle with plate {plate}.");
            Assert.True(auctionSystem.StartAuction(plate), $"Failed to start auction for vehicle with plate {plate}.");
            Assert.True(auctionSystem.PlaceBid(plate, "User1", 16000), $"Failed to place bid on auction for vehicle with plate {plate}.");
            int auctionResult = auctionSystem.EndAuction(plate, true);
            Assert.Equal(16000, auctionResult);
        }
    }
}
