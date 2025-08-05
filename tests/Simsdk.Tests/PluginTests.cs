using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Moq;
using SimSDK;
using SimSDK.Interfaces;
using SimSDK.Converters;
using Xunit;
using Model = SimSDK.Models;
using Rpc = Simsdkrpc;
using SimSDK.Tests;

namespace SimSDK.Tests
{
    public class PluginTests
    {
        [Fact]
        public async Task GetManifest_CallsPluginMethod()
        {
            // Arrange
            var manifest = new Model.Manifest { Name = "TestManifest", Version = "1.0" };
            var mockPlugin = new Mock<IPluginWithHandlers>(MockBehavior.Strict);
            mockPlugin.Setup(p => p.GetManifest()).Returns(manifest);

            var service = new GrpcAdapter(mockPlugin.Object);

            // Act
            var result = await service.GetManifest(new Rpc.ManifestRequest(), new TestServerCallContext());

            // Assert
            Assert.NotNull(result.Manifest);
            Assert.Equal("TestManifest", result.Manifest.Name);
            mockPlugin.Verify(p => p.GetManifest(), Times.Once);
        }

        [Fact]
        public async Task CreateComponentInstance_CallsPluginMethod()
        {
            // Arrange
            var request = new Rpc.CreateComponentRequest
            {
                ComponentType = "TypeA",
                ComponentId = "Comp1"
            };

            var mockPlugin = new Mock<IPluginWithHandlers>(MockBehavior.Strict);
            mockPlugin.Setup(p => p.CreateComponentInstance(It.IsAny<Model.CreateComponentRequest>()));

            var service = new GrpcAdapter(mockPlugin.Object);

            // Act
            var result = await service.CreateComponentInstance(request, new TestServerCallContext());

            // Assert
            Assert.NotNull(result);
            mockPlugin.Verify(p => p.CreateComponentInstance(It.IsAny<Model.CreateComponentRequest>()), Times.Once);
        }

        [Fact]
        public async Task DestroyComponentInstance_CallsPluginMethod()
        {
            // Arrange
            var request = new Google.Protobuf.WellKnownTypes.StringValue { Value = "CompToDestroy" };

            var mockPlugin = new Mock<IPluginWithHandlers>(MockBehavior.Strict);
            mockPlugin.Setup(p => p.DestroyComponentInstance("CompToDestroy"));

            var service = new GrpcAdapter(mockPlugin.Object);

            // Act
            var result = await service.DestroyComponentInstance(request, new TestServerCallContext());

            // Assert
            Assert.NotNull(result);
            mockPlugin.Verify(p => p.DestroyComponentInstance("CompToDestroy"), Times.Once);
        }

        [Fact]
        public async Task HandleMessage_ReturnsOutboundMessages()
        {
            // Arrange
            var request = new Rpc.SimMessage
            {
                MessageType = "MsgType",
                MessageId = "123",
                ComponentId = "CompX"
            };

            var outboundList = new List<Model.SimMessage>
            {
                new Model.SimMessage
                {
                    MessageType = "OutType",
                    MessageId = "456",
                    ComponentId = "CompY"
                }
            };

            var mockPlugin = new Mock<IPluginWithHandlers>(MockBehavior.Strict);
            mockPlugin.Setup(p => p.HandleMessage(It.IsAny<Model.SimMessage>())).Returns(outboundList);

            var service = new GrpcAdapter(mockPlugin.Object);

            // Act
            var result = await service.HandleMessage(request, new TestServerCallContext());

            // Assert
            Assert.Single(result.OutboundMessages);
            Assert.Equal("OutType", result.OutboundMessages[0].MessageType);
            mockPlugin.Verify(p => p.HandleMessage(It.IsAny<Model.SimMessage>()), Times.Once);
        }

        [Fact]
        public async Task MessageStream_ProcessesInit_And_Shutdown()
        {
            // Arrange
            var initEnv = new Rpc.PluginMessageEnvelope
            {
                Init = new Rpc.PluginInit { ComponentId = "InitComp" }
            };
            var shutdownEnv = new Rpc.PluginMessageEnvelope
            {
                Shutdown = new Rpc.PluginShutdown { Reason = "Done" }
            };

            var envelopes = new List<Rpc.PluginMessageEnvelope> { initEnv, shutdownEnv };

            var mockHandler = new Mock<IStreamHandler>(MockBehavior.Strict);
            mockHandler.Setup(h => h.OnInit(It.IsAny<Model.PluginInit>()));
            mockHandler.Setup(h => h.OnShutdown("Done"));

            var mockPlugin = new Mock<IPluginWithHandlers>(MockBehavior.Strict);
            mockPlugin.Setup(p => p.GetStreamHandler()).Returns(mockHandler.Object);

            var service = new GrpcAdapter(mockPlugin.Object);

            var reader = new TestAsyncStreamReader<Rpc.PluginMessageEnvelope>(envelopes);
            var writer = new TestServerStreamWriter<Rpc.PluginMessageEnvelope>();
            var context = new TestServerCallContext();

            // Act
            await service.MessageStream(reader, writer, context);

            // Assert
            mockHandler.Verify(h => h.OnInit(It.IsAny<Model.PluginInit>()), Times.Once);
            mockHandler.Verify(h => h.OnShutdown("Done"), Times.Once);
        }

        [Fact]
        public async Task MessageStream_ProcessesSimMessage_And_WritesAck()
        {
            // Arrange
            var simMsgEnv = new Rpc.PluginMessageEnvelope
            {
                SimMessage = new Rpc.SimMessage
                {
                    MessageType = "Type1",
                    MessageId = "Msg1",
                    ComponentId = "Comp1"
                }
            };
            var shutdownEnv = new Rpc.PluginMessageEnvelope
            {
                Shutdown = new Rpc.PluginShutdown { Reason = "End" }
            };

            var envelopes = new List<Rpc.PluginMessageEnvelope> { simMsgEnv, shutdownEnv };

            var handlerResponses = new List<Model.SimMessage>
            {
                new Model.SimMessage
                {
                    MessageType = "RespType",
                    MessageId = "Resp1",
                    ComponentId = "CompResp"
                }
            };

            var mockHandler = new Mock<IStreamHandler>(MockBehavior.Strict);
            mockHandler.Setup(h => h.OnSimMessage(It.IsAny<Model.SimMessage>())).Returns(handlerResponses);
            mockHandler.Setup(h => h.OnShutdown("End"));

            var mockPlugin = new Mock<IPluginWithHandlers>(MockBehavior.Strict);
            mockPlugin.Setup(p => p.GetStreamHandler()).Returns(mockHandler.Object);

            var service = new GrpcAdapter(mockPlugin.Object);

            var reader = new TestAsyncStreamReader<Rpc.PluginMessageEnvelope>(envelopes);
            var writer = new TestServerStreamWriter<Rpc.PluginMessageEnvelope>();
            var context = new TestServerCallContext();

            // Act
            await service.MessageStream(reader, writer, context);

            // Assert
            Assert.Contains(writer.Written, env => env.SimMessage?.MessageId == "Resp1");
            mockHandler.Verify(h => h.OnSimMessage(It.IsAny<Model.SimMessage>()), Times.Once);
            mockHandler.Verify(h => h.OnShutdown("End"), Times.Once);
        }
    }
}