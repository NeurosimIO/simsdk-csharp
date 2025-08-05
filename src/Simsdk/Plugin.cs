using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Rpc = Simsdkrpc;
using Model = SimSDK.Models;
using SimSDK.Converters;
using SimSDK.Interfaces;

namespace SimSDK
{
    /// <summary>
    /// Core plugin stream-handling logic, equivalent to Go's plugin.go.
    /// Contains the ServeStream method used by the Adapter.
    /// </summary>
    public static class Plugin
    {
        /// <summary>
        /// ServeStream is the gRPC bidirectional stream loop for a plugin.
        /// It reads PluginMessageEnvelope messages from the incoming stream,
        /// dispatches them to the provided handler, and sends any responses.
        /// </summary>
        public static async Task ServeStream(
            IStreamHandler handler,
            IAsyncStreamReader<Rpc.PluginMessageEnvelope> requestStream,
            IServerStreamWriter<Rpc.PluginMessageEnvelope> responseStream,
            ServerCallContext context)
        {
            await foreach (var envelope in requestStream.ReadAllAsync(context.CancellationToken))
            {
                switch (envelope.ContentCase)
                {
                    case Rpc.PluginMessageEnvelope.ContentOneofCase.Init:
                        if (handler is IStreamSenderSetter setter)
                        {
                            setter.SetStreamSender(
                                new GrpcStreamSender(responseStream, envelope.Init.ComponentId ?? string.Empty));
                        }
                        var initModel = PluginInitConverter.FromProto(envelope.Init);
                        handler.OnInit(initModel);
                        break;

                    case Rpc.PluginMessageEnvelope.ContentOneofCase.SimMessage:
                        var sdkMsg = SimMessageConverter.FromProto(envelope.SimMessage);
                        var responses = handler.OnSimMessage(sdkMsg);
                        foreach (var resp in responses)
                        {
                            await responseStream.WriteAsync(new Rpc.PluginMessageEnvelope
                            {
                                SimMessage = SimMessageConverter.ToProto(resp)
                            });
                        }

                        // Always send ACK for processed message
                        await responseStream.WriteAsync(new Rpc.PluginMessageEnvelope
                        {
                            Ack = new Rpc.PluginAck
                            {
                                MessageId = envelope.SimMessage.MessageId ?? string.Empty
                            }
                        });
                        break;

                    case Rpc.PluginMessageEnvelope.ContentOneofCase.Shutdown:
                        handler.OnShutdown(envelope.Shutdown.Reason ?? string.Empty);
                        return;

                    default:
                        // Ignore unknown or None content
                        break;
                }
            }
        }

        /// <summary>
        /// Concrete gRPC implementation of IStreamSender.
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