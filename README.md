# CarAuction
Implements an MVP car auction system, in C#, that handles requests via API.


# Technical decisions

=> Thread-safety in the bidding
    Multiple users may try to bid at the same time. As the system grows, most robust techniques must be used, such as a internal or external queue, to avoid losing bids or information. Since this is a simple, extensivl MVP, I used a normal lock.

=> Factory only for vehicles
    Since Auction is very simple and has only 1 "type", I made AuctionSystem dependent on it. If it gets more complex, an factory can be also made out of it, to decouple AuctionSystems an Auction. 

=> ConcurrentDictionary vs simpler structures
    This structure holds 2 important properties that we want most of the time: thread-safety and the hashmap. Since hashset has no concurrent counterpart, I used dictionaries to get both of these properties at the price of a bit of memory. Still, I tried to make the best use of the extra memory as possible.

=> Parameter repetition vs code simplicity
    I use an singleton and factory patterns to maintain the code centralized and to decouple the auction from the vehicle types. However, the number of passed parameters is on the verge of becomming too extensive. Everytime a field is added, it needs to be added to 15 other places.
    If the list gets longer, a builder pattern may be required. However, to deal with concurrency on the Factory/Builder, there would be 3 options: 
        - A scoped factory scheme to make an association of 1 builder per build
        - ThreadLocal to protect the properties
        - Creating an Action that fills the properties, which is thread-safe

    All options seemed too complex for the "simple to understand and modify" requirement, so I will keep the parameters. The code is ready for an extension, if needed, in the future.

=> Vehicle uniqueness

    How do we define that is a vehicle is unique? It is possible to use the internal C#/.NET Guid. However, there is a possibility that different users insert the same car for auctioning, which will cause 2 instances to be created with different Guid's. It is common practice for countries to consider that the vehicle's plate is unique, so that we can do the same here. This way, we can manually block any further error by requiring uniqueness of the plate. Here, we consider a alphanumeric 8-character string to represent a plate.

=> Throw vs Console Message
    The requirements specific tells to raise errors in situations such as duplicate vehicle or trying to add an auctioned vehicle to a new auction. However, is not clear how to handle that errors. I opted to build try-catch handlers inside the function. That means that most of the time, they will return something like a boolean, even after raising the error. 
     
# Assumptions

=> Auction 
    The requirement states that "When starting an auction, verify that the vehicle exists in the inventory and is not already in an active auction." and "validate that the auction for the given vehicle...", which indicates an 1 to 1 relationship between auction and vehicles. That is what was implemented.

=> Finished auction
    There is no specific requirement determining what to do with finished auctions. For now, the system is discarding them, and removing the car from the inventory, when it was "sold". If there is a need to store finished auctions, an auxiliary structure can be created with minor changes.
