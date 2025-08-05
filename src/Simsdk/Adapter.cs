using System.Threading.Tasks;
using Grpc.Core;
using Model = SimSDK.Models;
using Rpc = Simsdkrpc;
using SimSDK.Converters;
using SimSDK.Interfaces;

namespace SimSDK
{
    public class GrpcAdapter : Rpc.PluginService.PluginServiceBase
    {
        private readonly IPluginWithHandlers _plugin;

        public GrpcAdapter(IPluginWithHandlers plugin)
        {
            _plugin = plugin;
        }

        public override Task<Rpc.ManifestResponse> GetManifest(Rpc.ManifestRequest request, ServerCallContext context)
        {
            var manifest = _plugin.GetManifest();
            return Task.FromResult(new Rpc.ManifestResponse
            {
                Manifest = ManifestConverter.ToProto(manifest)
            });
        }

        public override Task<Rpc.CreateComponentResponse> CreateComponentInstance(Rpc.CreateComponentRequest request, ServerCallContext context)
        {
            var sdkReq = CreateComponentRequestConverter.FromProto(request);
            _plugin.CreateComponentInstance(sdkReq);
            return Task.FromResult(new Rpc.CreateComponentResponse());
        }

        public override Task<Google.Protobuf.WellKnownTypes.Empty> DestroyComponentInstance(Google.Protobuf.WellKnownTypes.StringValue id, ServerCallContext context)
        {
            _plugin.DestroyComponentInstance(id.Value);
            return Task.FromResult(new Google.Protobuf.WellKnownTypes.Empty());
        }

        public override Task<Rpc.MessageResponse> HandleMessage(Rpc.SimMessage request, ServerCallContext context)
        {
            var inMsg = SimMessageConverter.FromProto(request);
            var outbound = _plugin.HandleMessage(inMsg);

            var response = new Rpc.MessageResponse();
            foreach (var outMsg in outbound)
            {
                response.OutboundMessages.Add(SimMessageConverter.ToProto(outMsg));
            }

            return Task.FromResult(response);
        }

        public override async Task MessageStream(
            IAsyncStreamReader<Rpc.PluginMessageEnvelope> requestStream,
            IServerStreamWriter<Rpc.PluginMessageEnvelope> responseStream,
            ServerCallContext context)
        {
            // Delegate stream handling to the Plugin class
            await Plugin.ServeStream(_plugin.GetStreamHandler(), requestStream, responseStream, context);
        }
    }
}