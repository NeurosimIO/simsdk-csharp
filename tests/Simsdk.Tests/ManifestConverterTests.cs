using Xunit;
using System.Collections.Generic;
using System.Linq;
using SimSDK.Models;
using SimSDK.Converters;
using Rpc = Simsdkrpc;
using ModelFieldType = SimSDK.Models.FieldType;
using RpcFieldType = Simsdkrpc.FieldType;

namespace SimSDK.Tests.Converters
{
    public class ManifestConverterTests
    {
        [Fact]
        public void ToProto_MapsAllTopLevelFields()
        {
            var model = new Manifest
            {
                Name = "TestManifest",
                Version = "1.0",
                MessageTypes = new List<MessageType>
                {
                    new MessageType
                    {
                        Id = "msg1",
                        DisplayName = "Message 1",
                        Description = "Description 1",
                        Fields = new List<FieldSpec>
                        {
                            new FieldSpec
                            {
                                Name = "field1",
                                Type = ModelFieldType.String,
                                Subtype = ModelFieldType.Int,
                                EnumValues = new List<string>{ "A", "B" },
                                Description = "Field desc",
                                Required = true,
                                Repeated = false,
                                ObjectFields = new List<FieldSpec>()
                            }
                        }
                    }
                },
                ControlFunctions = new List<ControlFunctionType>
                {
                    new ControlFunctionType
                    {
                        Id = "cf1",
                        DisplayName = "Control 1",
                        Description = "CF Desc",
                        Fields = new List<FieldSpec>()
                    }
                },
                ComponentTypes = new List<ComponentType>
                {
                    new ComponentType
                    {
                        Id = "comp1",
                        DisplayName = "Component 1",
                        Description = "Comp Desc",
                        Internal = true,
                        SupportsMultipleInstances = false
                    }
                },
                TransportTypes = new List<TransportType>
                {
                    new TransportType
                    {
                        Id = "tcp",
                        DisplayName = "TCP",
                        Description = "Transport over TCP",
                        Internal = false
                    }
                }
            };

            var proto = ManifestConverter.ToProto(model);

            Assert.Equal(model.Name, proto.Name);
            Assert.Equal(model.Version, proto.Version);

            Assert.Single(proto.MessageTypes);
            var protoMsg = proto.MessageTypes.First();
            Assert.Equal("msg1", protoMsg.Id);
            Assert.Equal("Message 1", protoMsg.DisplayName);
            Assert.Equal("Description 1", protoMsg.Description);

            Assert.Single(protoMsg.Fields);
            var protoField = protoMsg.Fields.First();
            Assert.Equal("field1", protoField.Name);
            Assert.Equal(RpcFieldType.String, protoField.Type);
            Assert.Equal(RpcFieldType.Int, protoField.Subtype);
            Assert.Equal(new List<string> { "A", "B" }, protoField.EnumValues);
            Assert.Equal("Field desc", protoField.Description);
            Assert.True(protoField.Required);
            Assert.False(protoField.Repeated);

            Assert.Single(proto.ControlFunctions);
            Assert.Equal("cf1", proto.ControlFunctions.First().Id);

            Assert.Single(proto.ComponentTypes);
            Assert.Equal("comp1", proto.ComponentTypes.First().Id);
            Assert.True(proto.ComponentTypes.First().Internal);
            Assert.False(proto.ComponentTypes.First().SupportsMultipleInstances);

            Assert.Single(proto.TransportTypes);
            Assert.Equal("tcp", proto.TransportTypes.First().Id);
        }

        [Fact]
        public void FromProto_MapsAllTopLevelFields()
        {
            var proto = new Rpc.Manifest
            {
                Name = "FromProtoManifest",
                Version = "2.0",
                MessageTypes =
                {
                    new Rpc.MessageType
                    {
                        Id = "msg2",
                        DisplayName = "Message 2",
                        Description = "Description 2",
                        Fields =
                        {
                            new Rpc.FieldSpec
                            {
                                Name = "field2",
                                Type = RpcFieldType.Enum,
                                Subtype = RpcFieldType.Bool,
                                EnumValues = { "X", "Y" },
                                Description = "Field2 desc",
                                Required = false,
                                Repeated = true
                            }
                        }
                    }
                },
                ControlFunctions =
                {
                    new Rpc.ControlFunctionType
                    {
                        Id = "cf2",
                        DisplayName = "Control 2",
                        Description = "CF2 Desc"
                    }
                },
                ComponentTypes =
                {
                    new Rpc.ComponentType
                    {
                        Id = "comp2",
                        DisplayName = "Component 2",
                        Description = "Comp2 Desc",
                        Internal = false,
                        SupportsMultipleInstances = true
                    }
                },
                TransportTypes =
                {
                    new Rpc.TransportType
                    {
                        Id = "udp",
                        DisplayName = "UDP",
                        Description = "Transport over UDP",
                        Internal = true
                    }
                }
            };

            var model = ManifestConverter.FromProto(proto);

            Assert.Equal(proto.Name, model.Name);
            Assert.Equal(proto.Version, model.Version);

            Assert.Single(model.MessageTypes);
            var msg = model.MessageTypes.First();
            Assert.Equal("msg2", msg.Id);
            Assert.Equal("Message 2", msg.DisplayName);
            Assert.Equal("Description 2", msg.Description);

            Assert.Single(msg.Fields);
            var field = msg.Fields.First();
            Assert.Equal("field2", field.Name);
            Assert.Equal(ModelFieldType.Enum, field.Type);
            Assert.Equal(ModelFieldType.Bool, field.Subtype);
            Assert.Equal(new List<string> { "X", "Y" }, field.EnumValues);
            Assert.Equal("Field2 desc", field.Description);
            Assert.False(field.Required);
            Assert.True(field.Repeated);

            Assert.Single(model.ControlFunctions);
            Assert.Equal("cf2", model.ControlFunctions.First().Id);

            Assert.Single(model.ComponentTypes);
            Assert.Equal("comp2", model.ComponentTypes.First().Id);
            Assert.False(model.ComponentTypes.First().Internal);
            Assert.True(model.ComponentTypes.First().SupportsMultipleInstances);

            Assert.Single(model.TransportTypes);
            Assert.Equal("udp", model.TransportTypes.First().Id);
        }

        [Fact]
        public void ToProto_EmptyLists_ProducesEmptyProtoLists()
        {
            var model = new Manifest
            {
                Name = "EmptyTest",
                Version = "0.0"
            };

            var proto = ManifestConverter.ToProto(model);

            Assert.Empty(proto.MessageTypes);
            Assert.Empty(proto.ControlFunctions);
            Assert.Empty(proto.ComponentTypes);
            Assert.Empty(proto.TransportTypes);
        }

        [Fact]
        public void FromProto_EmptyLists_ProducesEmptyModelLists()
        {
            var proto = new Rpc.Manifest
            {
                Name = "EmptyTest",
                Version = "0.0"
            };

            var model = ManifestConverter.FromProto(proto);

            Assert.Empty(model.MessageTypes);
            Assert.Empty(model.ControlFunctions);
            Assert.Empty(model.ComponentTypes);
            Assert.Empty(model.TransportTypes);
        }
    }
}