using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Moq;
using Xunit;
using Model = SimSDK.Models;
using Rpc = Simsdkrpc;
using SimSDK.Converters;
using SimSDK.Interfaces;

namespace SimSDK.Tests
{
    public class AdapterTests
    {
        [Fact]
        public async Task GetManifest_ReturnsExpectedManifest()
        {
            // Arrange
            var manifest = new Model.Manifest { Name = "Test", Version = "1.0" };
            var pluginMock = new Mock<IPluginWithHandlers>(MockBehavior.Strict);
            pluginMock.Setup(p => p.GetManifest()).Returns(manifest);

            var adapter = new GrpcAdapter(pluginMock.Object);
            var ctx = new TestServerCallContext();

            // Act
            var result = await adapter.GetManifest(new Rpc.ManifestRequest(), ctx);

            // Assert
            Assert.Equal(manifest.Name, result.Manifest.Name);
            Assert.Equal(manifest.Version, result.Manifest.Version);
            pluginMock.Verify(p => p.GetManifest(), Times.Once);
        }

        [Fact]
        public async Task CreateComponentInstance_CallsPlugin()
        {
            // Arrange
            var modelReq = new Model.CreateComponentRequest
            {
                ComponentType = "Type",
                ComponentId = "Id",
                Parameters = new Dictionary<string, string>()
            };

            var rpcReq = CreateComponentRequestConverter.ToProto(modelReq);

            var pluginMock = new Mock<IPluginWithHandlers>(MockBehavior.Strict);
            pluginMock.Setup(p => p.CreateComponentInstance(It.IsAny<Model.CreateComponentRequest>()));

            var adapter = new GrpcAdapter(pluginMock.Object);
            var ctx = new TestServerCallContext();

            // Act
            await adapter.CreateComponentInstance(rpcReq, ctx);

            // Assert
            pluginMock.Verify(p => p.CreateComponentInstance(It.Is<Model.CreateComponentRequest>(r =>
                r.ComponentType == modelReq.ComponentType &&
                r.ComponentId == modelReq.ComponentId)), Times.Once);
        }

        [Fact]
        public async Task DestroyComponentInstance_CallsPlugin()
        {
            // Arrange
            var pluginMock = new Mock<IPluginWithHandlers>(MockBehavior.Strict);
            pluginMock.Setup(p => p.DestroyComponentInstance("Comp123"));

            var adapter = new GrpcAdapter(pluginMock.Object);
            var ctx = new TestServerCallContext();

            // Act
            await adapter.DestroyComponentInstance(new Google.Protobuf.WellKnownTypes.StringValue { Value = "Comp123" }, ctx);

            // Assert
            pluginMock.Verify(p => p.DestroyComponentInstance("Comp123"), Times.Once);
        }

        [Fact]
        public async Task HandleMessage_ReturnsOutboundMessages()
        {
            // Arrange
            var modelMsg = new Model.SimMessage
            {
                MessageType = "MT",
                MessageId = "MID",
                ComponentId = "CID",
                Payload = new byte[0],
                Metadata = new Dictionary<string, string>()
            };

            var outboundMsgs = new List<Model.SimMessage> { modelMsg };

            var pluginMock = new Mock<IPluginWithHandlers>(MockBehavior.Strict);
            pluginMock.Setup(p => p.HandleMessage(It.IsAny<Model.SimMessage>()))
                      .Returns(outboundMsgs);

            var adapter = new GrpcAdapter(pluginMock.Object);
            var ctx = new TestServerCallContext();

            // Act
            var result = await adapter.HandleMessage(SimMessageConverter.ToProto(modelMsg), ctx);

            // Assert
            Assert.Single(result.OutboundMessages);
            Assert.Equal(modelMsg.MessageId, result.OutboundMessages[0].MessageId);
            pluginMock.Verify(p => p.HandleMessage(It.IsAny<Model.SimMessage>()), Times.Once);
        }

        [Fact]
        public async Task MessageStream_HandlesInitAndShutdown()
        {
            // Arrange
            var initModel = new Model.PluginInit { ComponentId = "Comp123" };

            var initEnvelope = new Rpc.PluginMessageEnvelope
            {
                Init = PluginInitConverter.ToProto(initModel)
            };

            var shutdownEnvelope = new Rpc.PluginMessageEnvelope
            {
                Shutdown = new Rpc.PluginShutdown { Reason = "done" }
            };

            var handlerMock = new Mock<IStreamHandler>(MockBehavior.Strict);
            handlerMock.Setup(h => h.OnInit(It.IsAny<Model.PluginInit>()));
            handlerMock.Setup(h => h.OnShutdown("done"));

            var pluginMock = new Mock<IPluginWithHandlers>(MockBehavior.Strict);
            pluginMock.Setup(p => p.GetStreamHandler()).Returns(handlerMock.Object);

            var adapter = new GrpcAdapter(pluginMock.Object);
            var reader = new TestAsyncStreamReader<Rpc.PluginMessageEnvelope>(new[] { initEnvelope, shutdownEnvelope });
            var writer = new TestServerStreamWriter<Rpc.PluginMessageEnvelope>();
            var ctx = new TestServerCallContext();

            // Act
            await adapter.MessageStream(reader, writer, ctx);

            // Assert
            handlerMock.Verify(h => h.OnInit(It.Is<Model.PluginInit>(pi => pi.ComponentId == "Comp123")), Times.Once);
            handlerMock.Verify(h => h.OnShutdown("done"), Times.Once);
        }
    }
}