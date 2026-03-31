# CarAuction
Implements an MVP car auction system, in C#, that handles requests via API.


# Technical decisions

=> Parameter repetition vs code simplicity
    I use an singleton and factory patterns to maintain the code centralized and to decouple the auction from the vehicle types. However, the number of passed parameters is on the verge of becomming too extensive. If other vehicle properties are added, a builder pattern may be required. However, to deal with concurrency on the Factory/Builder, there would be 3 options: 
        - A scoped factory scheme to make an association of 1 builder per build
        - ThreadLocal to protect the properties
        - Creating an Action that fills the properties, which is thread-safe

    All options seemed too complex for the "simple to understand and modify" requirement, so I will keep the parameters. The code is ready for an extension, if needed, in the future.
     