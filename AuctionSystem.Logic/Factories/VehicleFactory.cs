using AuctionSystem.Logic.Models.Vehicles;

namespace AuctionSystem.Logic.Factories;

public static class VehicleFactory
{
    public static Vehicle CreateHatchback(string manufacturer, int year, int startingBid, int numberOfDoors)
    {
        return new Hatchback(manufacturer, year, startingBid, numberOfDoors);
    }

    public static Vehicle CreateSedan(string manufacturer, int year, int startingBid, int numberOfDoors)
    {
        return new Sedan(manufacturer, year, startingBid, numberOfDoors);
    }

    public static Vehicle CreateSUV(string manufacturer, int year, int startingBid, int numberOfSeats)
    {
        return new SUV(manufacturer, year, startingBid, numberOfSeats);
    }

    public static Vehicle CreateTruck(string manufacturer, int year, int startingBid, int loadCapacity)
    {
        return new Truck(manufacturer, year, startingBid, loadCapacity);
    }

}