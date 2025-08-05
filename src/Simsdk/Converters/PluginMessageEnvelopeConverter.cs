using Rpc = Simsdkrpc;
using Model = SimSDK.Models;

namespace SimSDK.Converters
{
    public static class PluginMessageEnvelopeConverter
    {
        public static Rpc.PluginMessageEnvelope ToProto(Model.PluginMessageEnvelope model)
        {
            var proto = new Rpc.PluginMessageEnvelope();

            if (model.SimMessage != null)
            {
                proto.SimMessage = SimMessageConverter.ToProto(model.SimMessage);
            }
            else if (model.Ack != null)
            {
                proto.Ack = PluginAckConverter.ToProto(model.Ack);
            }
            else if (model.Nak != null)
            {
                proto.Nak = PluginNakConverter.ToProto(model.Nak);
            }
            else if (model.Init != null)
            {
                proto.Init = PluginInitConverter.ToProto(model.Init);
            }
            else if (model.Shutdown != null)
            {
                proto.Shutdown = PluginShutdownConverter.ToProto(model.Shutdown);
            }

            return proto;
        }

        public static Model.PluginMessageEnvelope FromProto(Rpc.PluginMessageEnvelope proto) => proto.ContentCase switch
        {
            Rpc.PluginMessageEnvelope.ContentOneofCase.SimMessage => new Model.PluginMessageEnvelope
            {
                SimMessage = SimMessageConverter.FromProto(proto.SimMessage)
            },
            Rpc.PluginMessageEnvelope.ContentOneofCase.Ack => new Model.PluginMessageEnvelope
            {
                Ack = PluginAckConverter.FromProto(proto.Ack)
            },
            Rpc.PluginMessageEnvelope.ContentOneofCase.Nak => new Model.PluginMessageEnvelope
            {
                Nak = PluginNakConverter.FromProto(proto.Nak)
            },
            Rpc.PluginMessageEnvelope.ContentOneofCase.Init => new Model.PluginMessageEnvelope
            {
                Init = PluginInitConverter.FromProto(proto.Init)
            },
            Rpc.PluginMessageEnvelope.ContentOneofCase.Shutdown => new Model.PluginMessageEnvelope
            {
                Shutdown = PluginShutdownConverter.FromProto(proto.Shutdown)
            },
            _ => new Model.PluginMessageEnvelope() // None or unknown
        };
    }
}