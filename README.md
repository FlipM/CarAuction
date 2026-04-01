# CarAuction
Implements a POC for a car auction system in C#. It is extensible for various applications, such as a RESTful API, by the addition of an integration layer.

# Building requirements
 
The code only requires .NET 10 framework. Useful commands:
    Build >> dotnet build
    Execute simple tests >> dotnet run --project .\AuctionSystem.App\
    Execute unit tests >> dotnet tests
Test suites can be isolated by using the "--filter" keyword.

# Funcionalities

The auction system allows the user to:
    - Add vehicles to the system's inventory.
    - Start a bid for a non-auctioned vehicle.
    - Place a bid for a vehicle with an active auction.
    - Finish an auction by either selling the vehicle or canceling the auction.
    - Search for vehicles in the inventory, filtered by model, year, type, and manufacturer.
    - Get the list of active auctions.
    - Print the log of bids on an auction.

The system does not allow yet (out of scope):
    - Removing a vehicle from the system
    - Registering multiple vehicles per auction
    - Search auctions, per vehicle


# Technical decisions

=> Thread-safety in the bidding
    Multiple users may try to bid simultaneously. As the system grows, the most robust techniques must be used, such as an internal or external queue, to avoid losing bids or information. Since this is a simple, extensive POC, I used a normal lock.

=> Factory only for vehicles
    Since Auction is very simple and has only 1 "type", I made AuctionSystem dependent on it. If it gets more complex, a factory can also be made out of it, to decouple AuctionSystems from Auction. 

=> ConcurrentDictionary vs simpler structures
    This structure has two important properties that we want most of the time: thread safety and the hash property. Since HashSet has no concurrent counterpart, I used dictionaries to get both of these properties at the price of a bit of memory. Still, I tried to make the best use of the extra memory as much as possible.

=> Parameter repetition vs code simplicity
    I use singleton and factory patterns to maintain the code centralized and to decouple the auction from the vehicle types. However, the number of passed parameters is on the verge of becoming too extensive. Every time a field is added, we need to update 15 other code locations.
    If the list gets longer, a builder pattern may be required. However, to deal with concurrency on the Factory/Builder, there would be 3 options: 
    - A scoped factory scheme to make an association of 1 builder per build
    - ThreadLocal to protect the properties
    - Creating an Action that fills the properties, which is thread-safe

    All options seemed too complex for the "simple to understand and modify" requirement, so I will keep the parameters. The code is ready for an extension to the builder pattern, if needed.

=> Vehicle uniqueness
    How do we define that a vehicle is unique? It is possible to use the internal C#/.NET Guid. However, there is a possibility that different users insert the same car for auctioning, which will cause 2 instances to be created with different Guids. It is common practice for countries to consider the vehicle's plate unique, so that we can do the same here. This way, we can manually block any further errors by requiring uniqueness of the plate. Here, we consider an alphanumeric 8-character string to represent a plate.

=> Throw vs Console Message
    The requirements specifically tell us to raise errors in situations such as duplicate vehicles or trying to add an auctioned vehicle to a new auction. However, it is not clear how to handle those errors. I've decided that I may also print the error message, depending on the situation. 
     
=> Verbose
    Code has a high level of verbosity. Any successful or failed attempt has a confirmation message. This is the standard for API's and is good for logging, but can be omitted in the future.

# Assumptions

=> Auction 
    The requirement states that "When starting an auction, verify that the vehicle exists in the inventory and is not already in an active auction," and "validate that the auction for the given vehicle...", which indicates a 1-to-1 relationship between auction and vehicles. That is what was implemented.

=> Finished auction
    There is no specific requirement determining what to do with finished auctions. For now, the system discards them and removes the car from the inventory when it was "sold". If there is a need to store finished auctions, an auxiliary structure can be created with minor changes.

=> Requirement inconsistencies
	There were some typing inconsistencies in the specifications, such as car type "sudan" and "Each vehicle has a type (Sedan, SUV, or Truck)" while the initial section also mentioned Hatchback. None of these caused real ambiguity, so I proceeded with the implementation that felt correct.
    
