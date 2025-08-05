using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using SimSDK;
using SimSDK.Interfaces;
using SimSDK.Tests;
using Xunit;
using Model = SimSDK.Models;
using Rpc = Simsdkrpc;

namespace SimSDK.Tests
{
    public class ServeStreamTests
    {
        [Fact]
        public async Task MessageStream_ProcessesInit_And_Shutdown()
        {
            // Arrange test envelopes
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

            var adapter = new GrpcAdapter(mockPlugin.Object);

            var reader = new TestAsyncStreamReader<Rpc.PluginMessageEnvelope>(envelopes);
            var writer = new TestServerStreamWriter<Rpc.PluginMessageEnvelope>();
            var context = new TestServerCallContext();

            // Act
            await adapter.MessageStream(reader, writer, context);

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

            var adapter = new GrpcAdapter(mockPlugin.Object);

            var reader = new TestAsyncStreamReader<Rpc.PluginMessageEnvelope>(envelopes);
            var writer = new TestServerStreamWriter<Rpc.PluginMessageEnvelope>();
            var context = new TestServerCallContext();

            // Act
            await adapter.MessageStream(reader, writer, context);

            // Assert
            Assert.Contains(writer.Written, env => env.SimMessage?.MessageId == "Resp1");
            mockHandler.Verify(h => h.OnSimMessage(It.IsAny<Model.SimMessage>()), Times.Once);
            mockHandler.Verify(h => h.OnShutdown("End"), Times.Once);
        }
    }
}