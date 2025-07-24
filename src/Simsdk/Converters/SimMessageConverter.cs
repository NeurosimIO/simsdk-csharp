using System;
using System.Collections.Generic;
using System.Linq;
using Rpc = Simsdkrpc;
using Model = SimSDK.Models;

namespace SimSDK.Converters
{
    public static class SimMessageConverter
    {
        public static Rpc.SimMessage ToProto(Model.SimMessage message)
        {
            return new Rpc.SimMessage
            {
                MessageType = message.MessageType,
                MessageId = message.MessageId,
                ComponentId = message.ComponentId,
                Payload = Google.Protobuf.ByteString.CopyFrom(message.Payload ?? Array.Empty<byte>()),
                Metadata = { message.Metadata ?? new Dictionary<string, string>() }
            };
        }

        public static Model.SimMessage FromProto(Rpc.SimMessage proto)
        {
            return new Model.SimMessage
            {
                MessageType = proto.MessageType,
                MessageId = proto.MessageId,
                ComponentId = proto.ComponentId,
                Payload = proto.Payload.ToByteArray(),
                Metadata = proto.Metadata.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            };
        }
    }
}