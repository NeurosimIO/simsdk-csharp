// File: FieldTypeConverter.cs

using Simsdkrpc;
using SimSDK.Models;

using ModelFieldType = SimSDK.Models.FieldType;
using RpcFieldType = Simsdkrpc.FieldType;

namespace Simsdk.Converters
{
    public static class FieldTypeConverter
    {
        public static SimSDK.Models.FieldType FromProto(this Simsdkrpc.FieldType proto) => proto switch
        {
            RpcFieldType.String => ModelFieldType.String,
            RpcFieldType.Int => ModelFieldType.Int,
            RpcFieldType.Uint => ModelFieldType.Uint,
            RpcFieldType.Float => ModelFieldType.Float,
            RpcFieldType.Bool => ModelFieldType.Bool,
            RpcFieldType.Enum => ModelFieldType.Enum,
            RpcFieldType.Timestamp => ModelFieldType.Timestamp,
            RpcFieldType.Repeated => ModelFieldType.Repeated,
            RpcFieldType.Object => ModelFieldType.Object,
            _ => ModelFieldType.Unspecified
        };

        public static RpcFieldType ToProto(this ModelFieldType ft) => ft switch
        {
            ModelFieldType.String => RpcFieldType.String,
            ModelFieldType.Int => RpcFieldType.Int,
            ModelFieldType.Uint => RpcFieldType.Uint,
            ModelFieldType.Float => RpcFieldType.Float,
            ModelFieldType.Bool => RpcFieldType.Bool,
            ModelFieldType.Enum => RpcFieldType.Enum,
            ModelFieldType.Timestamp => RpcFieldType.Timestamp,
            ModelFieldType.Repeated => RpcFieldType.Repeated,
            ModelFieldType.Object => RpcFieldType.Object,
            _ => RpcFieldType.Unspecified
        };
    }
}