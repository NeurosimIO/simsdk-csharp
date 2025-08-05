using Xunit;
using System.Collections.Generic;
using System.Linq;
using SimSDK.Models;
using SimSDK.Converters;
using Rpc = Simsdkrpc;

namespace SimSDK.Tests.Converters
{
    public class MessageResponseConverterTests
    {
        [Fact]
        public void ToProto_MapsAllOutboundMessages()
        {
            // Arrange - Static Model with known SimMessages
            var model = new MessageResponse
            {
                OutboundMessages = new List<SimMessage>
                {
                    new SimMessage
                    {
                        MessageType = "Alert",
                        MessageId = "M1",
                        ComponentId = "C1",
                        Payload = new byte[] { 1 },
                        Metadata = new Dictionary<string, string> { { "k1", "v1" } }
                    },
                    new SimMessage
                    {
                        MessageType = "Status",
                        MessageId = "M2",
                        ComponentId = "C2",
                        Payload = new byte[] { 2 },
                        Metadata = new Dictionary<string, string> { { "k2", "v2" } }
                    }
                }
            };

            // Act
            var proto = MessageResponseConverter.ToProto(model);

            // Assert
            Assert.Equal(2, proto.OutboundMessages.Count);
            Assert.Equal("Alert", proto.OutboundMessages[0].MessageType);
            Assert.Equal("M1", proto.OutboundMessages[0].MessageId);
            Assert.Equal("Status", proto.OutboundMessages[1].MessageType);
            Assert.Equal("M2", proto.OutboundMessages[1].MessageId);
        }

        [Fact]
        public void FromProto_MapsAllOutboundMessages()
        {
            // Arrange - Static Proto with known SimMessages
            var proto = new Rpc.MessageResponse
            {
                OutboundMessages =
                {
                    new Rpc.SimMessage
                    {
                        MessageType = "ProtoAlert",
                        MessageId = "PM1",
                        ComponentId = "PC1",
                        Payload = Google.Protobuf.ByteString.CopyFrom(new byte[] { 9 }),
                        Metadata = { { "pk1", "pv1" } }
                    },
                    new Rpc.SimMessage
                    {
                        MessageType = "ProtoStatus",
                        MessageId = "PM2",
                        ComponentId = "PC2",
                        Payload = Google.Protobuf.ByteString.CopyFrom(new byte[] { 8 }),
                        Metadata = { { "pk2", "pv2" } }
                    }
                }
            };

            // Act
            var model = MessageResponseConverter.FromProto(proto);

            // Assert
            Assert.Equal(2, model.OutboundMessages.Count);
            Assert.Equal("ProtoAlert", model.OutboundMessages[0].MessageType);
            Assert.Equal("PM1", model.OutboundMessages[0].MessageId);
            Assert.Equal("ProtoStatus", model.OutboundMessages[1].MessageType);
            Assert.Equal("PM2", model.OutboundMessages[1].MessageId);
        }

        [Fact]
        public void ToProto_NullOutboundMessages_UsesEmptyList()
        {
            // Arrange
            var model = new MessageResponse
            {
                
            };

            // Act
            var proto = MessageResponseConverter.ToProto(model);

            // Assert
            Assert.NotNull(proto.OutboundMessages);
            Assert.Empty(proto.OutboundMessages);
        }

        [Fact]
        public void FromProto_NullOutboundMessages_UsesEmptyList()
        {
            // Arrange
            var proto = new Rpc.MessageResponse
            {
                OutboundMessages = { } // Empty
            };

            // Act
            var model = MessageResponseConverter.FromProto(proto);

            // Assert
            Assert.NotNull(model.OutboundMessages);
            Assert.Empty(model.OutboundMessages);
        }

        [Fact]
        public void ToProto_And_FromProto_WithStaticData_AreIndividuallyCorrect()
        {
            // Model → Proto
            var model = new MessageResponse
            {
                OutboundMessages = new List<SimMessage>
                {
                    new SimMessage
                    {
                        MessageType = "StaticMT",
                        MessageId = "StaticID",
                        ComponentId = "StaticComp",
                        Payload = new byte[] { 5 },
                        Metadata = new Dictionary<string, string> { { "mk", "mv" } }
                    }
                }
            };
            var proto = MessageResponseConverter.ToProto(model);
            Assert.Single(proto.OutboundMessages);
            Assert.Equal("StaticMT", proto.OutboundMessages[0].MessageType);
            Assert.Equal("StaticID", proto.OutboundMessages[0].MessageId);

            // Proto → Model
            var protoStatic = new Rpc.MessageResponse
            {
                OutboundMessages =
                {
                    new Rpc.SimMessage
                    {
                        MessageType = "PMT",
                        MessageId = "PID",
                        ComponentId = "PComp",
                        Payload = Google.Protobuf.ByteString.CopyFrom(new byte[] { 7 }),
                        Metadata = { { "pk", "pv" } }
                    }
                }
            };
            var modelFromProto = MessageResponseConverter.FromProto(protoStatic);
            Assert.Single(modelFromProto.OutboundMessages);
            Assert.Equal("PMT", modelFromProto.OutboundMessages[0].MessageType);
            Assert.Equal("PID", modelFromProto.OutboundMessages[0].MessageId);
        }
    }
}