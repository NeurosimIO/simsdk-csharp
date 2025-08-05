using System;
using System.Collections.Generic;

using Simsdk.Converters;

using RpcFieldType = Simsdkrpc.FieldType;


namespace SimSDK.Models
{
    public class Manifest
    {
        public required string Name { get; set; }
        public required string Version { get; set; }
        public List<MessageType> MessageTypes { get; set; } = new();
        public List<ControlFunctionType> ControlFunctions { get; set; } = new();
        public List<ComponentType> ComponentTypes { get; set; } = new();
        public List<TransportType> TransportTypes { get; set; } = new();
    }

    public class MessageType
    {
        public required string Id { get; set; }
        public required string DisplayName { get; set; }
        public required string Description { get; set; }
        public List<FieldSpec> Fields { get; set; } = new();
    }

    public class ControlFunctionType
    {
        public required string Id { get; set; }
        public required string DisplayName { get; set; }
        public required string Description { get; set; }
        public List<FieldSpec> Fields { get; set; } = new();
    }

    public class ComponentType
    {
        public required string Id { get; set; }
        public required string DisplayName { get; set; }
        public required string Description { get; set; }
        public bool Internal { get; set; }
        public bool SupportsMultipleInstances { get; set; }
    }

    public class TransportType
    {
        public required string Id { get; set; }
        public required string DisplayName { get; set; }
        public required string Description { get; set; }
        public bool Internal { get; set; }
    }

    public class FieldSpec
    {
        public required string Name { get; set; }
        public FieldType Type { get; set; }
        public bool Required { get; set; }
        public List<string> EnumValues { get; set; } = new();
        public bool Repeated { get; set; }
        public required string Description { get; set; }
        public FieldType Subtype { get; set; }
        public List<FieldSpec> ObjectFields { get; set; } = new();
    }

    public enum FieldType
    {
        Unspecified = 0,
        String = 1,
        Int = 2,
        Uint = 3,
        Float = 4,
        Bool = 5,
        Enum = 6,
        Timestamp = 7,
        Repeated = 8,
        Object = 9
    }

    public class CreateComponentRequest
    {
        public required string ComponentType { get; set; }
        public required string ComponentId { get; set; }
        public Dictionary<string, string> Parameters { get; set; } = new();
    }

    public class SimMessage
    {
        public required string MessageType { get; set; }
        public required string MessageId { get; set; }
        public required string ComponentId { get; set; }
        public byte[]? Payload { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new();
    }

    public class MessageResponse
    {
        public List<SimMessage> OutboundMessages { get; set; } = new();
    }

    public class PluginAck
    {

        public string? MessageId { get; set; }
    }
    public class PluginNak
    {
        public string? MessageId { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class PluginInit
    {
        public string ComponentId { get; set; } = string.Empty;
    }
    public class PluginShutdown
    {
        public string Reason { get; set; } = string.Empty;
    }
    
    public class PluginMessageEnvelope
{
    public SimMessage? SimMessage { get; set; }
    public PluginAck? Ack { get; set; }
    public PluginNak? Nak { get; set; }
    public PluginInit? Init { get; set; }
    public PluginShutdown? Shutdown { get; set; }
}


}