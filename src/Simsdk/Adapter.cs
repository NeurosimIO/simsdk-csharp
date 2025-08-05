using System;
using System.Collections.Generic;
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
            _plugin = plugin ?? throw new ArgumentNullException(nameof(plugin));
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

        public override Task MessageStream(
            IAsyncStreamReader<Rpc.PluginMessageEnvelope> requestStream,
            IServerStreamWriter<Rpc.PluginMessageEnvelope> responseStream,
            ServerCallContext context)
        {
            return ServeStream(_plugin.GetStreamHandler(), requestStream, responseStream, context);
        }

        /// <summary>
        /// Matches the Go ServeStream function for streaming message handling.
        /// </summary>
        internal static async Task ServeStream(
            IStreamHandler handler,
            IAsyncStreamReader<Rpc.PluginMessageEnvelope> requestStream,
            IServerStreamWriter<Rpc.PluginMessageEnvelope> responseStream,
            ServerCallContext context)
        {
            await foreach (var incoming in requestStream.ReadAllAsync(context.CancellationToken))
            {
                switch (incoming.ContentCase)
                {
                    case Rpc.PluginMessageEnvelope.ContentOneofCase.Init:
                        if (handler is IStreamSenderSetter setter)
                        {
                            setter.SetStreamSender(new GrpcStreamSender(responseStream, incoming.Init.ComponentId));
                        }
                        handler.OnInit(PluginInitConverter.FromProto(incoming.Init));
                        break;

                    case Rpc.PluginMessageEnvelope.ContentOneofCase.SimMessage:
                        var sdkMsg = SimMessageConverter.FromProto(incoming.SimMessage);
                        var responses = handler.OnSimMessage(sdkMsg);
                        foreach (var resp in responses)
                        {
                            await responseStream.WriteAsync(new Rpc.PluginMessageEnvelope
                            {
                                SimMessage = SimMessageConverter.ToProto(resp)
                            });
                        }
                        await responseStream.WriteAsync(new Rpc.PluginMessageEnvelope
                        {
                            Ack = new Rpc.PluginAck { MessageId = incoming.SimMessage.MessageId }
                        });
                        break;

                    case Rpc.PluginMessageEnvelope.ContentOneofCase.Shutdown:
                        handler.OnShutdown(incoming.Shutdown.Reason);
                        return;

                    default:
                        // Ignore unknown or None content
                        break;
                }
            }
        }

        /// <summary>
        /// Internal implementation of IStreamSender for gRPC streams.
        /// </summary>
        private class GrpcStreamSender : IStreamSender
        {
            private readonly IServerStreamWriter<Rpc.PluginMessageEnvelope> _stream;
            private readonly string _componentId;

            public GrpcStreamSender(IServerStreamWriter<Rpc.PluginMessageEnvelope> stream, string componentId)
            {
                _stream = stream ?? throw new ArgumentNullException(nameof(stream));
                _componentId = componentId ?? string.Empty;
            }

            public Task Send(Model.SimMessage message)
            {
                return _stream.WriteAsync(new Rpc.PluginMessageEnvelope
                {
                    SimMessage = SimMessageConverter.ToProto(message)
                });
            }

            public string ComponentId() => _componentId;
        }
    }
}