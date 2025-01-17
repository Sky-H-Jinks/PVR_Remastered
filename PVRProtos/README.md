# PVR Protos

## Overview

The **PVRProtos** project serves as a centralized repository for all the **.proto** files used in the PVR ecosystem. These proto files define the gRPC service contracts, ensuring consistent communication between services and clients. The goal of this project is to maintain tightly coupled and synchronized `.proto` files, so that both the PVR client (e.g., .NET MAUI app) and the PVRServer (gRPC server) always use the most up-to-date contract definitions, avoiding versioning mismatches.

By centralizing the proto files in this project, we can streamline the process of managing service definitions and ensure that any updates are reflected across all related services automatically.

## Structure

The project is structured around the concept of defining **one proto file per "type" of endpoint**. This means that each service should have its own dedicated proto file, which should include the following:

- **Service Definition:** Defines the methods exposed by the service.
- **Messages:** Defines the input and output messages for each method.

This approach simplifies the management of the proto files, avoids duplication, and ensures that each service has a clear, isolated contract definition. It also makes it easier to update and version each endpoint independently while maintaining compatibility between the client and server.

## Guidelines

1. **One Service, One Proto File:** 
   - Each gRPC service should have a single `.proto` file.
   - The file should contain all the relevant service definitions and messages needed for that service type.

2. **Consistency in Naming Conventions:**
   - Ensure that the naming of your proto files follows a consistent pattern, such as `ServiceName.proto`.
   - Service names and methods should also follow the naming conventions to ensure clarity and ease of understanding.

3. **Avoid Duplication:**
   - If a message or service definition is shared across multiple services, consider defining it in a separate `.proto` file and importing it where necessary.

4. **Versioning:**
   - Whenever a breaking change is made to a service or message structure, the version of the proto file should be updated appropriately. This ensures backward compatibility and makes it easy to track changes.

## Usage

To integrate these proto files into your project, simply add this repository as a submodule or dependency and reference the `.proto` files in your gRPC client and server projects.

Example (gRPC client in .NET MAUI):

```csharp
// Example code to load a proto file for a gRPC client
var channel = GrpcChannel.ForAddress("https://localhost:5001");
var client = new MyGrpcService.MyGrpcServiceClient(channel);
```

## Contributing

If you wish to contribute to this project, please follow these guidelines:

- Ensure that any new service or endpoint is defined in its own `.proto` file.
- Follow the naming conventions and message structure guidelines outlined above.
- Write tests for any new service definitions or methods.
- Make sure to update documentation and versioning appropriately when introducing breaking changes.
