using Xunit;
using SimSDK.Models;
using SimSDK.Converters;
using Rpc = Simsdkrpc;
using System.Collections.Generic;

namespace SimSDK.Tests.Converters
{
    public class PluginMessageEnvelopeConverterTests
    {
        [Fact]
        public void ToProto_WithSimMessage_MapsCorrectly()
        {
            var model = new PluginMessageEnvelope
            {
                SimMessage = new SimMessage
                {
                    MessageType = "TestType",
                    MessageId = "123",
                    ComponentId = "Comp1",
                    Payload = new byte[] { 1, 2, 3 },
                    Metadata = new Dictionary<string, string> { { "k", "v" } }
                }
            };

            var proto = PluginMessageEnvelopeConverter.ToProto(model);

            Assert.Equal(Rpc.PluginMessageEnvelope.ContentOneofCase.SimMessage, proto.ContentCase);
            Assert.Equal("TestType", proto.SimMessage.MessageType);
        }

        [Fact]
        public void ToProto_WithAck_MapsCorrectly()
        {
            var model = new PluginMessageEnvelope
            {
                Ack = new PluginAck { MessageId = "Ack123" }
            };

            var proto = PluginMessageEnvelopeConverter.ToProto(model);

            Assert.Equal(Rpc.PluginMessageEnvelope.ContentOneofCase.Ack, proto.ContentCase);
            Assert.Equal("Ack123", proto.Ack.MessageId);
        }

        [Fact]
        public void ToProto_WithNak_MapsCorrectly()
        {
            var model = new PluginMessageEnvelope
            {
                Nak = new PluginNak { MessageId = "Nak123", ErrorMessage = "Error" }
            };

            var proto = PluginMessageEnvelopeConverter.ToProto(model);

            Assert.Equal(Rpc.PluginMessageEnvelope.ContentOneofCase.Nak, proto.ContentCase);
            Assert.Equal("Nak123", proto.Nak.MessageId);
            Assert.Equal("Error", proto.Nak.ErrorMessage);
        }

        [Fact]
        public void ToProto_WithInit_MapsCorrectly()
        {
            var model = new PluginMessageEnvelope
            {
                Init = new PluginInit { ComponentId = "InitComp" }
            };

            var proto = PluginMessageEnvelopeConverter.ToProto(model);

            Assert.Equal(Rpc.PluginMessageEnvelope.ContentOneofCase.Init, proto.ContentCase);
            Assert.Equal("InitComp", proto.Init.ComponentId);
        }

        [Fact]
        public void ToProto_WithShutdown_MapsCorrectly()
        {
            var model = new PluginMessageEnvelope
            {
                Shutdown = new PluginShutdown { Reason = "Shutting down" }
            };

            var proto = PluginMessageEnvelopeConverter.ToProto(model);

            Assert.Equal(Rpc.PluginMessageEnvelope.ContentOneofCase.Shutdown, proto.ContentCase);
            Assert.Equal("Shutting down", proto.Shutdown.Reason);
        }

        [Fact]
        public void FromProto_WithSimMessage_MapsCorrectly()
        {
            var proto = new Rpc.PluginMessageEnvelope
            {
                SimMessage = new Rpc.SimMessage { MessageType = "TypeA", MessageId = "1", ComponentId = "C1" }
            };

            var model = PluginMessageEnvelopeConverter.FromProto(proto);

            Assert.NotNull(model.SimMessage);
            Assert.Equal("TypeA", model.SimMessage.MessageType);
        }

        [Fact]
        public void FromProto_WithAck_MapsCorrectly()
        {
            var proto = new Rpc.PluginMessageEnvelope
            {
                Ack = new Rpc.PluginAck { MessageId = "AckID" }
            };

            var model = PluginMessageEnvelopeConverter.FromProto(proto);

            Assert.NotNull(model.Ack);
            Assert.Equal("AckID", model.Ack.MessageId);
        }

        [Fact]
        public void FromProto_WithNak_MapsCorrectly()
        {
            var proto = new Rpc.PluginMessageEnvelope
            {
                Nak = new Rpc.PluginNak { MessageId = "NakID", ErrorMessage = "Err" }
            };

            var model = PluginMessageEnvelopeConverter.FromProto(proto);

            Assert.NotNull(model.Nak);
            Assert.Equal("NakID", model.Nak.MessageId);
            Assert.Equal("Err", model.Nak.ErrorMessage);
        }

        [Fact]
        public void FromProto_WithInit_MapsCorrectly()
        {
            var proto = new Rpc.PluginMessageEnvelope
            {
                Init = new Rpc.PluginInit { ComponentId = "InitID" }
            };

            var model = PluginMessageEnvelopeConverter.FromProto(proto);

            Assert.NotNull(model.Init);
            Assert.Equal("InitID", model.Init.ComponentId);
        }

        [Fact]
        public void FromProto_WithShutdown_MapsCorrectly()
        {
            var proto = new Rpc.PluginMessageEnvelope
            {
                Shutdown = new Rpc.PluginShutdown { Reason = "ReasonX" }
            };

            var model = PluginMessageEnvelopeConverter.FromProto(proto);

            Assert.NotNull(model.Shutdown);
            Assert.Equal("ReasonX", model.Shutdown.Reason);
        }

        [Fact]
        public void ToProto_And_FromProto_Symmetric_WithSimMessage()
        {
            var original = new PluginMessageEnvelope
            {
                SimMessage = new SimMessage
                {
                    MessageType = "SymType",
                    MessageId = "Sym1",
                    ComponentId = "SymComp"
                }
            };

            var proto = PluginMessageEnvelopeConverter.ToProto(original);
            var roundTripped = PluginMessageEnvelopeConverter.FromProto(proto);

            Assert.NotNull(roundTripped.SimMessage); // ensures safe dereference
            Assert.Equal(original.SimMessage.MessageType, roundTripped.SimMessage!.MessageType);
        }
        
        [Fact]
        public void FromProto_NoneContent_ReturnsEmptyEnvelope()
        {
            // Arrange: create an Rpc.PluginMessageEnvelope with no content set
            var proto = new Simsdkrpc.PluginMessageEnvelope();

            // Act
            var result = PluginMessageEnvelopeConverter.FromProto(proto);

            // Assert
            Assert.Null(result.SimMessage);
            Assert.Null(result.Ack);
            Assert.Null(result.Nak);
            Assert.Null(result.Init);
            Assert.Null(result.Shutdown);
        }
    }
}