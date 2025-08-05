using Xunit;
using System.Collections.Generic;
using SimSDK.Models;
using SimSDK.Converters;
using Simsdkrpc;


namespace SimSDK.Tests.Converters
{
    public class CreateComponentRequestConverterTests
    {
        [Fact]
        public void ToProto_MapsAllFields()
        {
            // Arrange
            var model = new SimSDK.Models.CreateComponentRequest
            {
                ComponentType = "TestType",
                ComponentId = "Comp123",
                Parameters = new Dictionary<string, string>
                {
                    { "param1", "value1" },
                    { "param2", "value2" }
                }
            };

            // Act
            var proto = CreateComponentRequestConverter.ToProto(model);

            // Assert
            Assert.Equal(model.ComponentType, proto.ComponentType);
            Assert.Equal(model.ComponentId, proto.ComponentId);
            Assert.Equal(model.Parameters.Count, proto.Parameters.Count);
            Assert.Equal(model.Parameters["param1"], proto.Parameters["param1"]);
            Assert.Equal(model.Parameters["param2"], proto.Parameters["param2"]);
        }

        [Fact]
        public void FromProto_MapsAllFields()
        {
            // Arrange
            var proto = new Simsdkrpc.CreateComponentRequest
            {
                ComponentType = "TestType",
                ComponentId = "Comp123",
                Parameters =
                {
                    { "param1", "value1" },
                    { "param2", "value2" }
                }
            };

            // Act
            var model = CreateComponentRequestConverter.FromProto(proto);

            // Assert
            Assert.Equal(proto.ComponentType, model.ComponentType);
            Assert.Equal(proto.ComponentId, model.ComponentId);
            Assert.Equal(proto.Parameters.Count, model.Parameters.Count);
            Assert.Equal(proto.Parameters["param1"], model.Parameters["param1"]);
            Assert.Equal(proto.Parameters["param2"], model.Parameters["param2"]);
        }

        [Fact]
        public void ToProto_NullParameters_UsesEmptyDictionary()
        {
            // Arrange
            var model = new SimSDK.Models.CreateComponentRequest
            {
                ComponentType = "TestType",
                ComponentId = "Comp123"
            };

            // Act
            var proto = CreateComponentRequestConverter.ToProto(model);

            // Assert
            Assert.NotNull(proto.Parameters);
            Assert.Empty(proto.Parameters);
        }

        [Fact]
        public void FromProto_EmptyParameters_MapsToEmptyDictionary()
        {
            // Arrange
            var proto = new Simsdkrpc.CreateComponentRequest
            {
                ComponentType = "TestType",
                ComponentId = "Comp123"
                // No parameters set
            };

            // Act
            var model = CreateComponentRequestConverter.FromProto(proto);

            // Assert
            Assert.NotNull(model.Parameters);
            Assert.Empty(model.Parameters);
        }

        [Fact]
        public void ToProto_And_FromProto_AreSymmetric_ForStaticData()
        {
            // Arrange
            var original = new SimSDK.Models.CreateComponentRequest
            {
                ComponentType = "SymmetricType",
                ComponentId = "Sym123",
                Parameters = new Dictionary<string, string>
                {
                    { "k1", "v1" },
                    { "k2", "v2" }
                }
            };

            // Act
            var proto = CreateComponentRequestConverter.ToProto(original);
            var roundTripped = CreateComponentRequestConverter.FromProto(proto);

            // Assert
            Assert.Equal(original.ComponentType, roundTripped.ComponentType);
            Assert.Equal(original.ComponentId, roundTripped.ComponentId);
            Assert.Equal(original.Parameters, roundTripped.Parameters);
        }
    }
}