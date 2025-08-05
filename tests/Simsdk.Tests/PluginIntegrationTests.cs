using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;
using Rpc = Simsdkrpc;
using SimSDK;
using SimSDK.Interfaces;

public class PluginIntegrationTests
{
    [Fact]
    public async Task GetManifest_Works()
    {
        using var host = await CreateGrpcHost();
        var address = "http://localhost:5005";

        using var channel = GrpcChannel.ForAddress(address, new GrpcChannelOptions
        {
            HttpHandler = new SocketsHttpHandler
            {
                EnableMultipleHttp2Connections = true
            }
        });

        var client = new Rpc.PluginService.PluginServiceClient(channel);

        var manifestResp = await client.GetManifestAsync(new Rpc.ManifestRequest());
        Assert.NotNull(manifestResp.Manifest);
        Assert.Equal("TestManifest", manifestResp.Manifest.Name);
    }

    [Fact]
    public async Task HandleMessage_Works()
    {
        using var host = await CreateGrpcHost();
        var address = "http://localhost:5005";

        using var channel = GrpcChannel.ForAddress(address, new GrpcChannelOptions
        {
            HttpHandler = new SocketsHttpHandler
            {
                EnableMultipleHttp2Connections = true
            }
        });

        var client = new Rpc.PluginService.PluginServiceClient(channel);

        var simMsg = new Rpc.SimMessage
        {
            MessageType = "TestType",
            MessageId = "msg1",
            ComponentId = "comp1"
        };

        var response = await client.HandleMessageAsync(simMsg);
        Assert.Single(response.OutboundMessages);
        Assert.Equal("Reply", response.OutboundMessages[0].MessageType);
    }

    [Fact]
    public async Task MessageStream_Works()
    {
        using var host = await CreateGrpcHost();
        var address = "http://localhost:5005";

        using var channel = GrpcChannel.ForAddress(address, new GrpcChannelOptions
        {
            HttpHandler = new SocketsHttpHandler
            {
                EnableMultipleHttp2Connections = true
            }
        });

        var client = new Rpc.PluginService.PluginServiceClient(channel);

        using var call = client.MessageStream();

        // Send Init
        await call.RequestStream.WriteAsync(new Rpc.PluginMessageEnvelope
        {
            Init = new Rpc.PluginInit { ComponentId = "Comp1" }
        });

        // Send SimMessage
        await call.RequestStream.WriteAsync(new Rpc.PluginMessageEnvelope
        {
            SimMessage = new Rpc.SimMessage
            {
                MessageType = "Ping",
                MessageId = "123",
                ComponentId = "Comp1"
            }
        });

        // Send Shutdown
        await call.RequestStream.WriteAsync(new Rpc.PluginMessageEnvelope
        {
            Shutdown = new Rpc.PluginShutdown { Reason = "Done" }
        });

        await call.RequestStream.CompleteAsync();

        // Read all responses
        var responses = new List<Rpc.PluginMessageEnvelope>();
        while (await call.ResponseStream.MoveNext(default))
        {
            responses.Add(call.ResponseStream.Current);
        }

        // Assert we got a SimMessage reply and an Ack
        Assert.Contains(responses, r => r.SimMessage?.MessageType == "Reply");
        Assert.Contains(responses, r => r.Ack?.MessageId == "123");
    }

    private async Task<IHost> CreateGrpcHost()
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseKestrel(options =>
                {
                    options.ListenLocalhost(5005, listenOptions =>
                    {
                        listenOptions.Protocols = HttpProtocols.Http2;
                    });
                })
                .ConfigureServices(services =>
                {
                    services.AddGrpc();
                    services.AddSingleton<IPluginWithHandlers, DummyPlugin>();
                })
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGrpcService<GrpcAdapter>();
                    });
                });
            })
            .Build();

        await host.StartAsync();
        return host;
    }

    private class DummyPlugin : IPluginWithHandlers
    {
        public SimSDK.Models.Manifest GetManifest() => new SimSDK.Models.Manifest
        {
            Name = "TestManifest",
            Version = "1.0"
        };

        public void CreateComponentInstance(SimSDK.Models.CreateComponentRequest request) { }

        public void DestroyComponentInstance(string componentId) { }

        public List<SimSDK.Models.SimMessage> HandleMessage(SimSDK.Models.SimMessage message)
            => new()
            {
                new SimSDK.Models.SimMessage
                {
                    MessageType = "Reply",
                    MessageId = "resp1",
                    ComponentId = "compResp"
                }
            };

        public IStreamHandler GetStreamHandler() => new DummyStreamHandler();

        private class DummyStreamHandler : IStreamHandler
        {
            public List<SimSDK.Models.SimMessage> OnSimMessage(SimSDK.Models.SimMessage message)
                => new()
                {
                    new SimSDK.Models.SimMessage
                    {
                        MessageType = "Reply",
                        MessageId = "resp1",
                        ComponentId = "compResp"
                    }
                };

            public void OnInit(SimSDK.Models.PluginInit init) { }

            public void OnShutdown(string reason) { }
        }
    }
}