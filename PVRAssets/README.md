# PVRAssets

PVRAssets is a reusable, testable, and runnable .NET library designed to support the **PVR** .NET MAUI application. It contains all the core non-UI logic, services, and business rules, ensuring separation of concerns and enabling comprehensive testing without the need for platform-specific dependencies.

## Features

- **Business Logic:** Encapsulates the application's business rules and processes.
- **Services Layer:** Provides reusable and extendable service implementations.
- **Models:** Includes data models shared between the UI and back-end logic.
- **Background Services:** Handles operations that run on background threads, such as configuration retrieval via gRPC calls.
- **Logging:** Implements Serilog with a file sink and a custom gRPC sink (`GRPCBatchSink`) to batch and periodically send logged entries.
- **Networking:** Provides gRPC client code for efficient and reliable communication.

## Modules

### BackgroundServices

The `BackgroundServices` module handles operations that need to run on background threads. For instance:
- **Configuration Retrieval:** Implements periodic gRPC calls to fetch configuration data from the server efficiently.
- Designed for high reliability and extensibility.

### Logging

Logging in PVRAssets uses **Serilog** with the following features:
- **File Sink:** Logs messages to a local file for persistent storage.
- **GRPCBatchSink:** Custom sink that batches log entries and sends them to a gRPC endpoint periodically for server-side processing.

### Networking

The `Networking` module contains gRPC client code for all network interactions. This ensures:
- Efficient communication with the server.
- Strongly typed APIs for gRPC methods.