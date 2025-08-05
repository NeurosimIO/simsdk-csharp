using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using SimSDK.Models;
using SimSDK.Converters;
using Rpc = Simsdkrpc;

namespace SimSDK.Tests.Converters
{
    public class SimMessageConverterTests
    {
        [Fact]
        public void ToProto_MapsAllFields()
        {
            // Arrange - static Model data
            var model = new SimMessage
            {
                MessageType = "Alert",
                MessageId = "MSG-001",
                ComponentId = "Comp-XYZ",
                Payload = new byte[] { 1, 2, 3 },
                Metadata = new Dictionary<string, string>
                {
                    { "key1", "value1" },
                    { "key2", "value2" }
                }
            };

            // Act
            var proto = SimMessageConverter.ToProto(model);

            // Assert
            Assert.Equal("Alert", proto.MessageType);
            Assert.Equal("MSG-001", proto.MessageId);
            Assert.Equal("Comp-XYZ", proto.ComponentId);
            Assert.Equal(ByteString.CopyFrom(new byte[] { 1, 2, 3 }), proto.Payload);
            Assert.Equal(2, proto.Metadata.Count);
            Assert.Equal("value1", proto.Metadata["key1"]);
            Assert.Equal("value2", proto.Metadata["key2"]);
        }

        [Fact]
        public void FromProto_MapsAllFields()
        {
            // Arrange - static Proto data
            var proto = new Rpc.SimMessage
            {
                MessageType = "StatusUpdate",
                MessageId = "MSG-999",
                ComponentId = "Comp-ABC",
                Payload = ByteString.CopyFrom(new byte[] { 9, 8, 7 }),
                Metadata =
                {
                    { "m1", "meta1" },
                    { "m2", "meta2" }
                }
            };

            // Act
            var model = SimMessageConverter.FromProto(proto);

            // Assert
            Assert.Equal("StatusUpdate", model.MessageType);
            Assert.Equal("MSG-999", model.MessageId);
            Assert.Equal("Comp-ABC", model.ComponentId);
            Assert.Equal(new byte[] { 9, 8, 7 }, model.Payload);
            Assert.Equal(2, model.Metadata.Count);
            Assert.Equal("meta1", model.Metadata["m1"]);
            Assert.Equal("meta2", model.Metadata["m2"]);
        }

        [Fact]
        public void ToProto_NullPayloadAndMetadata_UsesDefaults()
        {
            // Arrange
            var model = new SimMessage
            {
                MessageType = "NoPayload",
                MessageId = "NP-001",
                ComponentId = "Comp-Empty",
                Payload = null
            };

            // Act
            var proto = SimMessageConverter.ToProto(model);

            // Assert
            Assert.NotNull(proto.Payload);
            Assert.Empty(proto.Payload.ToByteArray());
            Assert.NotNull(proto.Metadata);
            Assert.Empty(proto.Metadata);
        }

        [Fact]
        public void FromProto_NullMetadata_UsesEmptyDictionary()
        {
            // Arrange
            var proto = new Rpc.SimMessage
            {
                MessageType = "NoMeta",
                MessageId = "NM-001",
                ComponentId = "Comp-NoMeta",
                Payload = ByteString.CopyFrom(new byte[] { 4, 5, 6 }),
                Metadata = { } // Empty, but not null
            };

            // Act
            var model = SimMessageConverter.FromProto(proto);

            // Assert
            Assert.NotNull(model.Metadata);
            Assert.Empty(model.Metadata);
        }

        [Fact]
        public void ToProto_And_FromProto_AreNotSymmetricTests()
        {
            // Arrange - Model to Proto static
            var model = new SimMessage
            {
                MessageType = "TestMT",
                MessageId = "ID-123",
                ComponentId = "CompTest",
                Payload = new byte[] { 42 },
                Metadata = new Dictionary<string, string> { { "alpha", "beta" } }
            };

            // Act
            var proto = SimMessageConverter.ToProto(model);

            // Assert
            Assert.Equal("TestMT", proto.MessageType);
            Assert.Equal("ID-123", proto.MessageId);
            Assert.Equal("CompTest", proto.ComponentId);
            Assert.Equal(ByteString.CopyFrom(new byte[] { 42 }), proto.Payload);
            Assert.Single(proto.Metadata);
            Assert.Equal("beta", proto.Metadata["alpha"]);

            // Arrange - Proto to Model static
            var protoStatic = new Rpc.SimMessage
            {
                MessageType = "FromProtoMT",
                MessageId = "FP-456",
                ComponentId = "CompProto",
                Payload = ByteString.CopyFrom(new byte[] { 99 }),
                Metadata = { { "k", "v" } }
            };

            // Act
            var modelFromProto = SimMessageConverter.FromProto(protoStatic);

            // Assert
            Assert.Equal("FromProtoMT", modelFromProto.MessageType);
            Assert.Equal("FP-456", modelFromProto.MessageId);
            Assert.Equal("CompProto", modelFromProto.ComponentId);
            Assert.Equal(new byte[] { 99 }, modelFromProto.Payload);
            Assert.Single(modelFromProto.Metadata);
            Assert.Equal("v", modelFromProto.Metadata["k"]);
        }
    }
}