using System.Collections.Generic;
using System.Linq;
using Rpc = Simsdkrpc;
using Model = SimSDK.Models;

namespace SimSDK.Converters
{
    public static class ManifestConverter
    {
        public static Rpc.Manifest ToProto(Model.Manifest manifest)
        {
            return new Rpc.Manifest
            {
                Name = manifest.Name,
                Version = manifest.Version,
                MessageTypes = { manifest.MessageTypes?.Select(ToProto).ToList() ?? new() },
                ControlFunctions = { manifest.ControlFunctions?.Select(ToProto).ToList() ?? new() },
                ComponentTypes = { manifest.ComponentTypes?.Select(ToProto).ToList() ?? new() },
                TransportTypes = { manifest.TransportTypes?.Select(ToProto).ToList() ?? new() }
            };
        }

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
            Description = type.Description
        };

        public static Rpc.TransportType ToProto(Model.TransportType type) => new()
        {
            Id = type.Id,
            DisplayName = type.DisplayName,
            Description = type.Description
        };

        public static Rpc.FieldSpec ToProto(Model.FieldSpec modelField) => new()
        {
            Name = modelField.Name,
            Type = (Rpc.FieldType)modelField.Type,
            Subtype = (Rpc.FieldType)modelField.Subtype,
            EnumValues = { modelField.EnumValues ?? new() },
            Description = modelField.Description,
            ObjectFields = { (modelField.ObjectFields ?? new()).Select(ToProto) }
        };
    }
}