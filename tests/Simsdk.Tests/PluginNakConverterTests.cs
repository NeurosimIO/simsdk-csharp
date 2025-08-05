using Xunit;
using SimSDK.Models;
using SimSDK.Converters;
using Rpc = Simsdkrpc;

namespace SimSDK.Tests.Converters
{
    public class PluginNakConverterTests
    {
        [Fact]
        public void ToProto_MapsAllFields()
        {
            // Arrange
            var model = new PluginNak
            {
                MessageId = "msg123",
                ErrorMessage = "Something went wrong"
            };

            // Act
            var proto = PluginNakConverter.ToProto(model);

            // Assert
            Assert.Equal("msg123", proto.MessageId);
            Assert.Equal("Something went wrong", proto.ErrorMessage);
        }

        [Fact]
        public void ToProto_NullFields_DefaultsToEmptyStrings()
        {
            // Arrange
            var model = new PluginNak
            {
                MessageId = null
        
            };

            // Act
            var proto = PluginNakConverter.ToProto(model);

            // Assert
            Assert.Equal(string.Empty, proto.MessageId);
            Assert.Equal(string.Empty, proto.ErrorMessage);
        }

        [Fact]
        public void FromProto_MapsAllFields()
        {
            // Arrange
            var proto = new Rpc.PluginNak
            {
                MessageId = "proto-msg",
                ErrorMessage = "proto-error"
            };

            // Act
            var model = PluginNakConverter.FromProto(proto);

            // Assert
            Assert.Equal("proto-msg", model.MessageId);
            Assert.Equal("proto-error", model.ErrorMessage);
        }

        [Fact]
        public void FromProto_EmptyStrings_MapsCorrectly()
        {
            // Arrange
            var proto = new Rpc.PluginNak
            {
                MessageId = string.Empty,
                ErrorMessage = string.Empty
            };

            // Act
            var model = PluginNakConverter.FromProto(proto);

            // Assert
            Assert.Equal(string.Empty, model.MessageId);
            Assert.Equal(string.Empty, model.ErrorMessage);
        }

        [Fact]
        public void ToProto_And_FromProto_StaticData_Symmetric()
        {
            // Arrange
            var original = new PluginNak
            {
                MessageId = "static-msg",
                ErrorMessage = "static-error"
            };

            // Act
            var proto = PluginNakConverter.ToProto(original);
            var roundTripped = PluginNakConverter.FromProto(proto);

            // Assert
            Assert.Equal(original.MessageId, roundTripped.MessageId);
            Assert.Equal(original.ErrorMessage, roundTripped.ErrorMessage);
        }
    }
}