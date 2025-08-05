using Rpc = Simsdkrpc;
using Model = SimSDK.Models;

namespace SimSDK.Converters
{
    public static class PluginShutdownConverter
    {
        public static Rpc.PluginShutdown ToProto(Model.PluginShutdown model) => new Rpc.PluginShutdown
        {
            Reason = model.Reason ?? string.Empty
        };

        public static Model.PluginShutdown FromProto(Rpc.PluginShutdown proto) => new Model.PluginShutdown
        {
            Reason = proto.Reason ?? string.Empty
        };
    }
}