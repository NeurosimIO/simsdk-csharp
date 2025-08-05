using System.Collections.Generic;
using System.Linq;
using Rpc = Simsdkrpc;
using Model = SimSDK.Models;
using ModelFieldType = SimSDK.Models.FieldType;
using RpcFieldType = Simsdkrpc.FieldType;
using SimSDK.Converters;
using Simsdk.Converters; // For FieldTypeConverter

namespace SimSDK.Converters
{
    public static class ManifestConverter
    {
        // Model → Rpc
        public static Rpc.Manifest ToProto(Model.Manifest manifest) => new()
        {
            Name = manifest.Name,
            Version = manifest.Version,
            MessageTypes = { manifest.MessageTypes?.Select(ToProto).ToList() ?? new() },
            ControlFunctions = { manifest.ControlFunctions?.Select(ToProto).ToList() ?? new() },
            ComponentTypes = { manifest.ComponentTypes?.Select(ToProto).ToList() ?? new() },
            TransportTypes = { manifest.TransportTypes?.Select(ToProto).ToList() ?? new() }
        };

        public static Rpc.MessageType ToProto(Model.MessageType type) => new()
        {
            Id = type.Id,
            DisplayName = type.DisplayName,
            Description = type.Description,
            Fields = { type.Fields?.Select(ToProto).ToList() ?? new() }
        };

        public static Rpc.ControlFunctionType ToProto(Model.ControlFunctionType type) => new()
        {
            Id = type.Id,
            DisplayName = type.DisplayName,
            Description = type.Description,
            Fields = { type.Fields?.Select(ToProto).ToList() ?? new() }
        };

        public static Rpc.ComponentType ToProto(Model.ComponentType type) => new()
        {
            Id = type.Id,
            DisplayName = type.DisplayName,
            Description = type.Description,
            Internal = type.Internal,
            SupportsMultipleInstances = type.SupportsMultipleInstances
        };

        public static Rpc.TransportType ToProto(Model.TransportType type) => new()
        {
            Id = type.Id,
            DisplayName = type.DisplayName,
            Description = type.Description,
            Internal = type.Internal
        };

        public static Rpc.FieldSpec ToProto(Model.FieldSpec modelField) => new()
        {
            Name = modelField.Name,
            Type = FieldTypeConverter.ToProto(modelField.Type),
            Subtype = FieldTypeConverter.ToProto(modelField.Subtype),
            EnumValues = { modelField.EnumValues ?? new() },
            Description = modelField.Description,
            ObjectFields = { modelField.ObjectFields?.Select(ToProto) ?? new List<Rpc.FieldSpec>() },
            Required = modelField.Required,
            Repeated = modelField.Repeated
        };

        // Rpc → Model
        public static Model.Manifest FromProto(Rpc.Manifest proto) => new()
        {
            Name = proto.Name,
            Version = proto.Version,
            MessageTypes = proto.MessageTypes.Select(FromProto).ToList(),
            ControlFunctions = proto.ControlFunctions.Select(FromProto).ToList(),
            ComponentTypes = proto.ComponentTypes.Select(FromProto).ToList(),
            TransportTypes = proto.TransportTypes.Select(FromProto).ToList()
        };

        public static Model.MessageType FromProto(Rpc.MessageType proto) => new()
        {
            Id = proto.Id,
            DisplayName = proto.DisplayName,
            Description = proto.Description,
            Fields = proto.Fields.Select(FromProto).ToList()
        };

        public static Model.ControlFunctionType FromProto(Rpc.ControlFunctionType proto) => new()
        {
            Id = proto.Id,
            DisplayName = proto.DisplayName,
            Description = proto.Description,
            Fields = proto.Fields.Select(FromProto).ToList()
        };

        public static Model.ComponentType FromProto(Rpc.ComponentType proto) => new()
        {
            Id = proto.Id,
            DisplayName = proto.DisplayName,
            Description = proto.Description,
            Internal = proto.Internal,
            SupportsMultipleInstances = proto.SupportsMultipleInstances
        };

        public static Model.TransportType FromProto(Rpc.TransportType proto) => new()
        {
            Id = proto.Id,
            DisplayName = proto.DisplayName,
            Description = proto.Description,
            Internal = proto.Internal
        };

        public static Model.FieldSpec FromProto(Rpc.FieldSpec proto) => new()
        {
            Name = proto.Name,
            Type = FieldTypeConverter.FromProto(proto.Type),
            Subtype = FieldTypeConverter.FromProto(proto.Subtype),
            EnumValues = proto.EnumValues.ToList(),
            Description = proto.Description,
            ObjectFields = proto.ObjectFields.Select(FromProto).ToList(),
            Required = proto.Required,
            Repeated = proto.Repeated
        };
    }
}