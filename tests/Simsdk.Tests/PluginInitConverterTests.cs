using Xunit;
using SimSDK.Models;
using SimSDK.Converters;
using Rpc = Simsdkrpc;

namespace SimSDK.Tests.Converters
{
    public class PluginInitConverterTests
    {
        [Fact]
        public void ToProto_MapsAllFields()
        {
            var model = new PluginInit { ComponentId = "comp-123" };
            var proto = PluginInitConverter.ToProto(model);

            Assert.Equal("comp-123", proto.ComponentId);
        }

        [Fact]
        public void ToProto_NullComponentId_MapsToEmptyString()
        {
            var model = new PluginInit {};
            var proto = PluginInitConverter.ToProto(model);

            Assert.Equal(string.Empty, proto.ComponentId);
        }

        [Fact]
        public void FromProto_MapsAllFields()
        {
            var proto = new Rpc.PluginInit { ComponentId = "proto-comp-456" };
            var model = PluginInitConverter.FromProto(proto);

            Assert.Equal("proto-comp-456", model.ComponentId);
        }

        [Fact]
        public void FromProto_EmptyStringComponentId_MapsToEmptyString()
        {
            var proto = new Rpc.PluginInit { ComponentId = string.Empty };
            var model = PluginInitConverter.FromProto(proto);

            Assert.Equal(string.Empty, model.ComponentId);
        }

        [Fact]
        public void ToProto_And_FromProto_StaticData_Symmetric()
        {
            var original = new PluginInit { ComponentId = "sym-test" };
            var proto = PluginInitConverter.ToProto(original);
            var roundTripped = PluginInitConverter.FromProto(proto);

            Assert.Equal(original.ComponentId, roundTripped.ComponentId);
        }
    }
}