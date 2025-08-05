using Rpc = Simsdkrpc;
using Model = SimSDK.Models;

namespace SimSDK.Converters
{
    public static class PluginAckConverter
    {
        public static Rpc.PluginAck ToProto(Model.PluginAck model) => new()
        {
            MessageId = model.MessageId ?? string.Empty
        };

        public static Model.PluginAck FromProto(Rpc.PluginAck proto) => new()
        {
            MessageId = proto.MessageId
        };
    }
}