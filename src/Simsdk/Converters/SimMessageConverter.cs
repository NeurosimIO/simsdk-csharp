using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Rpc = Simsdkrpc;
using Model = SimSDK.Models;

namespace SimSDK.Converters
{
    public static class SimMessageConverter
    {
        public static Rpc.SimMessage ToProto(Model.SimMessage message) => new Rpc.SimMessage
        {
            MessageType = message.MessageType ?? string.Empty,
            MessageId = message.MessageId ?? string.Empty,
            ComponentId = message.ComponentId ?? string.Empty,
            Payload = ByteString.CopyFrom(message.Payload ?? Array.Empty<byte>()),
            Metadata = { message.Metadata ?? new Dictionary<string, string>() }
        };

        public static Model.SimMessage FromProto(Rpc.SimMessage proto) => new Model.SimMessage
        {
            MessageType = proto.MessageType ?? string.Empty,
            MessageId = proto.MessageId ?? string.Empty,
            ComponentId = proto.ComponentId ?? string.Empty,
            Payload = proto.Payload?.ToByteArray(),
            Metadata = proto.Metadata?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? new Dictionary<string, string>()
        };
    }
}