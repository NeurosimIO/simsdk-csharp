using Rpc = Simsdkrpc;
using Model = SimSDK.Models;

namespace SimSDK.Converters
{
    public static class PluginNakConverter
    {
        public static Rpc.PluginNak ToProto(Model.PluginNak nak) => new Rpc.PluginNak
        {
            MessageId = nak.MessageId ?? string.Empty,
            ErrorMessage = nak.ErrorMessage ?? string.Empty
        };

        public static Model.PluginNak FromProto(Rpc.PluginNak proto) => new Model.PluginNak
        {
            MessageId = proto.MessageId,
            ErrorMessage = proto.ErrorMessage
        };
    }
}