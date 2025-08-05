using Rpc = Simsdkrpc;
using Model = SimSDK.Models;

namespace SimSDK.Converters
{
    public static class PluginInitConverter
    {
        public static Rpc.PluginInit ToProto(Model.PluginInit init) => new()
        {
            ComponentId = init.ComponentId
        };

        public static Model.PluginInit FromProto(Rpc.PluginInit proto) => new()
        {
            ComponentId = proto.ComponentId
        };
    }
}