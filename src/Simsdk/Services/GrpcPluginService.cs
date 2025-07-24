using System.Threading.Tasks;
using Grpc.Core;
using SimSDK.Converters;
using SimSDK.Models;
using SimSDK.Interfaces;
using Rpc = Simsdkrpc;
using System.Linq;

namespace SimSDK.Services
{
    public class GrpcPluginService : Rpc.PluginService.PluginServiceBase
    {
        private readonly IPlugin _plugin;

        public GrpcPluginService(IPlugin plugin)
        {
            _plugin = plugin;
        }

        public override Task<Rpc.ManifestResponse> GetManifest(Rpc.ManifestRequest request, ServerCallContext context)
        {
            Manifest manifest = _plugin.GetManifest();
            var response = new Rpc.ManifestResponse
            {
                Manifest = ManifestConverter.ToProto(manifest)
            };
            return Task.FromResult(response);
        }

        public override Task<Rpc.CreateComponentResponse> CreateComponentInstance(Rpc.CreateComponentRequest request, ServerCallContext context)
        {
            var modelRequest = new CreateComponentRequest
            {
                ComponentType = request.ComponentType,
                ComponentId = request.ComponentId,
                Parameters = request.Parameters.ToDictionary(entry => entry.Key, entry => entry.Value)
            };

            _plugin.CreateComponentInstance(modelRequest);
            return Task.FromResult(new Rpc.CreateComponentResponse());
        }

        public override Task<Google.Protobuf.WellKnownTypes.Empty> DestroyComponentInstance(Rpc.DestroyComponentRequest request, ServerCallContext context)
        {
            _plugin.DestroyComponentInstance(request.ComponentId);
            return Task.FromResult(new Google.Protobuf.WellKnownTypes.Empty());
        }

        public override Task<Rpc.MessageResponse> HandleMessage(Rpc.SimMessage request, ServerCallContext context)
        {
            SimMessage simMessage = SimMessageConverter.FromProto(request);
            var responses = _plugin.HandleMessage(simMessage);
            var reply = new Rpc.MessageResponse();
            reply.OutboundMessages.AddRange(responses.ConvertAll(SimMessageConverter.ToProto));
            return Task.FromResult(reply);
        }
    }
}