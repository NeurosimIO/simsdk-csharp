using System.Linq;
using System.Threading.Tasks;
using Grpc.AspNetCore.Server;
using Microsoft.AspNetCore.Mvc;
using SimSDK.Converters;
using SimSDK.Interfaces;
using SimSDK.Models;
using Rpc = Simsdkrpc;

namespace SimSDK.Services
{
    public class GrpcPluginService : Rpc.PluginService.PluginServiceBase
    {
        private readonly IPlugin _plugin;

        public GrpcPluginService(IPlugin plugin)
        {
            _plugin = plugin;
        }

        public override Task<Rpc.ManifestResponse> GetManifest(Rpc.ManifestRequest request,
            Grpc.Core.ServerCallContext context)
        {
            var manifest = _plugin.GetManifest();
            var response = new Rpc.ManifestResponse
            {
                Manifest = ManifestConverter.ToProto(manifest)
            };
            return Task.FromResult(response);
        }

        public override Task<Rpc.CreateComponentResponse> CreateComponentInstance(Rpc.CreateComponentRequest request,
            Grpc.Core.ServerCallContext context)
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

        public override Task<Google.Protobuf.WellKnownTypes.Empty> DestroyComponentInstance(
            Rpc.DestroyComponentRequest request, Grpc.Core.ServerCallContext context)
        {
            _plugin.DestroyComponentInstance(request.ComponentId);
            return Task.FromResult(new Google.Protobuf.WellKnownTypes.Empty());
        }

        public override Task<Rpc.MessageResponse> HandleMessage(Rpc.SimMessage request,
            Grpc.Core.ServerCallContext context)
        {
            var simMessage = SimMessageConverter.FromProto(request);
            var responses = _plugin.HandleMessage(simMessage);

            var reply = new Rpc.MessageResponse();
            reply.OutboundMessages.AddRange(responses.ConvertAll(SimMessageConverter.ToProto));

            return Task.FromResult(reply);
        }
    }
}