# Datum Carrier

## Overview

The **DatumCarrier** project is a revamped version of the original **DatumCarrier**. It consolidates all of the courier functionality spread across the original **DatumCarrier** and the carrier integrations in the original PVR system. This refactor aims to streamline carrier integrations into a more maintainable and flexible architecture, ensuring ease of use and scalability for future courier implementations.

The key goal of this project is to provide a unified structure for all carrier-related integrations, while adhering to best practices in software design.

## Directory Structure

The structure of the **DatumCarrier** project follows a strict organizational pattern to maintain clarity and separation of concerns. Carrier integrations should be implemented as follows:

```
Carrier/<Carrier Name>/<API Version>/<Implementation goes here>
```

Each carrier implementation should reside in its respective folder, structured by carrier name and API version. This makes it easier to manage and maintain different carrier versions over time.

### Example:
```
Carrier/FedEx/v1/CreateShipmentRequest.cs
Carrier/UPS/v1/CreateShipmentRequest.cs
Carrier/DHL/v2/CreateShipmentRequest.cs
```

- **Carrier Name:** The name of the courier service (e.g., FedEx, UPS, DHL).
- **API Version:** The version of the API being integrated (e.g., v1, v2, v3_3).
- **Implementation:** This is where the actual carrier integration logic goes. This should implement the required interfaces and handle all necessary communication and operations related to the specific carrier.

## Interface

Each carrier integration **must** implement the `ICarrier` interface. This ensures that all carrier integrations follow a consistent structure and can be interacted with in a uniform way.

```csharp
public interface ICarrier
{
    Task<ShippingLabel> GenerateShippingLabelAsync(Consignment nConsignment);
    Task<TrackingInfo> TrackShipmentAsync(string nTrackingNumber);
}
```

Implementing this interface allows for interoperability between various carrier integrations and ensures that methods like `GenerateShippingLabelAsync` and `TrackShipmentAsync` are implemented across all carriers.

## Guidelines

- **Carrier Implementations Only:**  
  All carrier-specific code should go into the `Carrier` directory or its subdirectories. Non-carrier specific logic should be placed outside the `Carrier` directory to maintain separation of concerns.

- **Folder Structure:**  
  Each carrier should have its own directory under the `Carrier` folder, and this should include the API versioning as needed. This ensures the system can handle multiple versions of a carrier’s API without conflicts.

- **Avoid Duplication:**  
  Any shared logic or utilities between different carriers should be placed in separate shared folders outside of the `Carrier` directory to avoid duplication.

## Usage

To integrate new carriers, simply create a new directory under `Carrier`, following the structure mentioned earlier. Implement the `ICarrier` interface and any necessary methods specific to the carrier.

Example (FedEx implementation):

```
Carrier/FedEx/v1/ShippingService.cs
```


```csharp
public class FedExCarrier : ICarrier
{
    public async Task<ShippingLabel> GenerateShippingLabelAsync(Consignment nConsignment)
    {
        // Implementation specific to FedEx API v1
    }

    public async Task<TrackingInfo> TrackShipmentAsync(string nTrackingNumber)
    {
        // Implementation specific to FedEx API v1
    }
}
```

## Contributing

If you'd like to contribute to the **DatumCarrier** project, follow these guidelines:

- Ensure that each carrier integration is placed in the appropriate directory structure.
- Implement the `ICarrier` interface for each new carrier.
- Non-carrier specific logic should not be added to the `Carrier` directory.
- Follow the coding conventions and write clear documentation for each carrier integration.
- Write tests in `PVRTESTS` for new implementations to ensure reliability and compatibility. This should follow a similar structure to the couriers.
