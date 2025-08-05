using System.Collections.Generic;
using System.Linq;
using Rpc = Simsdkrpc;
using Model = SimSDK.Models;

using ModelFieldType = SimSDK.Models.FieldType;
using RpcFieldType = Simsdkrpc.FieldType;

namespace SimSDK.Converters
{
    public static class CreateComponentRequestConverter
    {
        public static Rpc.CreateComponentRequest ToProto(Model.CreateComponentRequest request)
        {
            return new Rpc.CreateComponentRequest
            {
                ComponentType = request.ComponentType,
                ComponentId = request.ComponentId,
                Parameters = { request.Parameters ?? new Dictionary<string, string>() }
            };
        }

        public static Model.CreateComponentRequest FromProto(Rpc.CreateComponentRequest proto)
        {
            return new Model.CreateComponentRequest
            {
                ComponentType = proto.ComponentType,
                ComponentId = proto.ComponentId,
                Parameters = proto.Parameters.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            };
        }
    }
}