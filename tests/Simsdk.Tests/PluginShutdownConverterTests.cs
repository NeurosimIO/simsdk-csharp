using Xunit;
using SimSDK.Models;
using SimSDK.Converters;
using Rpc = Simsdkrpc;

namespace SimSDK.Tests.Converters
{
    public class PluginShutdownConverterTests
    {
        [Fact]
        public void ToProto_MapsAllFields()
        {
            var model = new PluginShutdown { Reason = "Normal shutdown" };
            var proto = PluginShutdownConverter.ToProto(model);

            Assert.Equal("Normal shutdown", proto.Reason);
        }

        [Fact]
        public void ToProto_NullReason_MapsToEmptyString()
        {
            var model = new PluginShutdown {};
            var proto = PluginShutdownConverter.ToProto(model);

            Assert.Equal(string.Empty, proto.Reason);
        }

        [Fact]
        public void FromProto_MapsAllFields()
        {
            var proto = new Rpc.PluginShutdown { Reason = "Proto shutdown" };
            var model = PluginShutdownConverter.FromProto(proto);

            Assert.Equal("Proto shutdown", model.Reason);
        }

        [Fact]
        public void FromProto_EmptyStringReason_MapsToEmptyString()
        {
            var proto = new Rpc.PluginShutdown { Reason = string.Empty };
            var model = PluginShutdownConverter.FromProto(proto);

            Assert.Equal(string.Empty, model.Reason);
        }

        [Fact]
        public void ToProto_And_FromProto_StaticData_Symmetric()
        {
            var original = new PluginShutdown { Reason = "Symmetric test" };
            var proto = PluginShutdownConverter.ToProto(original);
            var roundTripped = PluginShutdownConverter.FromProto(proto);

            Assert.Equal(original.Reason, roundTripped.Reason);
        }
    }
}