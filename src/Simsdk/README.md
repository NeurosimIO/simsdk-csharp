# SimSDK for C#

This is the official C# SDK for implementing NeuroSim-compatible simulation plugins.

## Overview

SimSDK provides base types, gRPC service stubs, and helper classes to enable plugin developers to create and register components that integrate with the [NeuroSim simulator-core](https://github.com/neurosimio/simulator-core-api).

This SDK is functionally equivalent to the [Go SDK](https://github.com/neurosimio/simsdk_go), and supports dynamic messaging, component instantiation, and transport adapters.

## Getting Started

Install the NuGet package:

```bash
dotnet add package SimSDK --version 0.0.16
```

Then implement the `IPlugin` interface in your plugin entrypoint:

```csharp
public class SamplePlugin : IPlugin
{
    public Manifest GetManifest() => new Manifest { /* ... */ };
    public void CreateComponentInstance(CreateComponentRequest request) { /* ... */ }
    public void DestroyComponentInstance(string componentId) { /* ... */ }
    public List<SimMessage> HandleMessage(SimMessage message) => new();
}
```

## Usage

Use this SDK to build a gRPC server that implements `PluginService`, responding to simulation messages and control events. See `GrpcPluginService` for an adapter implementation.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
