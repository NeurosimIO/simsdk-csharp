using Xunit;
using SimSDK.Models;
using SimSDK.Converters;
using Rpc = Simsdkrpc;

namespace SimSDK.Tests.Converters
{
    public class PluginAckConverterTests
    {
        [Fact]
        public void ToProto_MapsMessageId()
        {
            // Arrange - static Model
            var model = new PluginAck
            {
                MessageId = "Ack123"
            };

            // Act
            var proto = PluginAckConverter.ToProto(model);

            // Assert
            Assert.Equal("Ack123", proto.MessageId);
        }

        [Fact]
        public void ToProto_NullMessageId_UsesEmptyString()
        {
            // Arrange
            var model = new PluginAck
            {
                MessageId = null
            };

            // Act
            var proto = PluginAckConverter.ToProto(model);

            // Assert
            Assert.Equal(string.Empty, proto.MessageId);
        }

        [Fact]
        public void FromProto_MapsMessageId()
        {
            // Arrange - static Proto
            var proto = new Rpc.PluginAck
            {
                MessageId = "ProtoAck"
            };

            // Act
            var model = PluginAckConverter.FromProto(proto);

            // Assert
            Assert.Equal("ProtoAck", model.MessageId);
        }

        [Fact]
        public void FromProto_EmptyMessageId_MapsToEmptyString()
        {
            // Arrange
            var proto = new Rpc.PluginAck
            {
                MessageId = string.Empty // Protobuf-safe "unset" equivalent
            };

            // Act
            var model = PluginAckConverter.FromProto(proto);

            // Assert
            Assert.NotNull(model.MessageId); // Should never be null
            Assert.Equal(string.Empty, model.MessageId);
        }

        [Fact]
        public void ToProto_And_FromProto_WithStaticData_AreIndividuallyCorrect()
        {
            // Model → Proto
            var model = new PluginAck { MessageId = "ModelStatic" };
            var proto = PluginAckConverter.ToProto(model);
            Assert.Equal("ModelStatic", proto.MessageId);

            // Proto → Model
            var protoStatic = new Rpc.PluginAck { MessageId = "ProtoStatic" };
            var modelFromProto = PluginAckConverter.FromProto(protoStatic);
            Assert.Equal("ProtoStatic", modelFromProto.MessageId);
        }
    }
}