using Xunit;
using SimSDK.Models;
using SimSDK.Converters;
using Simsdkrpc;

using ModelFieldType = SimSDK.Models.FieldType;
using RpcFieldType = Simsdkrpc.FieldType;
using Simsdk.Converters;

namespace SimSDK.Tests.Converters
{
    public class FieldTypeConverterTests
    {
        [Theory]
        [InlineData(ModelFieldType.String, RpcFieldType.String)]
        [InlineData(ModelFieldType.Int, RpcFieldType.Int)]
        [InlineData(ModelFieldType.Uint, RpcFieldType.Uint)]
        [InlineData(ModelFieldType.Float, RpcFieldType.Float)]
        [InlineData(ModelFieldType.Bool, RpcFieldType.Bool)]
        [InlineData(ModelFieldType.Enum, RpcFieldType.Enum)]
        [InlineData(ModelFieldType.Timestamp, RpcFieldType.Timestamp)]
        [InlineData(ModelFieldType.Repeated, RpcFieldType.Repeated)]
        [InlineData(ModelFieldType.Object, RpcFieldType.Object)]
        [InlineData(ModelFieldType.Unspecified, RpcFieldType.Unspecified)]
        public void ToProto_StaticMapping_Works(ModelFieldType modelType, RpcFieldType expectedProto)
        {
            var proto = modelType.ToProto();
            Assert.Equal(expectedProto, proto);
        }

        [Theory]
        [InlineData(RpcFieldType.String, ModelFieldType.String)]
        [InlineData(RpcFieldType.Int, ModelFieldType.Int)]
        [InlineData(RpcFieldType.Uint, ModelFieldType.Uint)]
        [InlineData(RpcFieldType.Float, ModelFieldType.Float)]
        [InlineData(RpcFieldType.Bool, ModelFieldType.Bool)]
        [InlineData(RpcFieldType.Enum, ModelFieldType.Enum)]
        [InlineData(RpcFieldType.Timestamp, ModelFieldType.Timestamp)]
        [InlineData(RpcFieldType.Repeated, ModelFieldType.Repeated)]
        [InlineData(RpcFieldType.Object, ModelFieldType.Object)]
        [InlineData(RpcFieldType.Unspecified, ModelFieldType.Unspecified)]
        public void FromProto_StaticMapping_Works(RpcFieldType protoType, ModelFieldType expectedModel)
        {
            var model = protoType.FromProto();
            Assert.Equal(expectedModel, model);
        }

        [Theory]
        [InlineData(ModelFieldType.String, RpcFieldType.String)]
        [InlineData(ModelFieldType.Int, RpcFieldType.Int)]
        [InlineData(ModelFieldType.Uint, RpcFieldType.Uint)]
        [InlineData(ModelFieldType.Float, RpcFieldType.Float)]
        [InlineData(ModelFieldType.Bool, RpcFieldType.Bool)]
        [InlineData(ModelFieldType.Enum, RpcFieldType.Enum)]
        [InlineData(ModelFieldType.Timestamp, RpcFieldType.Timestamp)]
        [InlineData(ModelFieldType.Repeated, RpcFieldType.Repeated)]
        [InlineData(ModelFieldType.Object, RpcFieldType.Object)]
        [InlineData(ModelFieldType.Unspecified, RpcFieldType.Unspecified)]
        public void ToProto_KnownMapping_Then_FromProto(ModelFieldType modelType, RpcFieldType expectedProto)
        {
            var proto = modelType.ToProto();
            var decodedModel = proto.FromProto();

            Assert.Equal(expectedProto, proto);
            Assert.Equal(modelType, decodedModel);
        }

        [Theory]
        [InlineData(RpcFieldType.String, ModelFieldType.String)]
        [InlineData(RpcFieldType.Int, ModelFieldType.Int)]
        [InlineData(RpcFieldType.Uint, ModelFieldType.Uint)]
        [InlineData(RpcFieldType.Float, ModelFieldType.Float)]
        [InlineData(RpcFieldType.Bool, ModelFieldType.Bool)]
        [InlineData(RpcFieldType.Enum, ModelFieldType.Enum)]
        [InlineData(RpcFieldType.Timestamp, ModelFieldType.Timestamp)]
        [InlineData(RpcFieldType.Repeated, ModelFieldType.Repeated)]
        [InlineData(RpcFieldType.Object, ModelFieldType.Object)]
        [InlineData(RpcFieldType.Unspecified, ModelFieldType.Unspecified)]
        public void FromProto_KnownMapping_Then_ToProto(RpcFieldType protoType, ModelFieldType expectedModel)
        {
            var model = protoType.FromProto();
            var encodedProto = model.ToProto();

            Assert.Equal(expectedModel, model);
            Assert.Equal(protoType, encodedProto);
        }

        // -------- Negative tests --------

        [Fact]
        public void ToProto_InvalidModelType_MapsToUnspecified()
        {
            var invalidModel = (ModelFieldType)999; // not in enum
            var proto = invalidModel.ToProto();
            Assert.Equal(RpcFieldType.Unspecified, proto);
        }

        [Fact]
        public void FromProto_InvalidProtoType_MapsToUnspecified()
        {
            var invalidProto = (RpcFieldType)999; // not in enum
            var model = invalidProto.FromProto();
            Assert.Equal(ModelFieldType.Unspecified, model);
        }
    }
}