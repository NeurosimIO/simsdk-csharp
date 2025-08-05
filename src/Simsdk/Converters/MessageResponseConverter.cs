using System.Linq;
using Rpc = Simsdkrpc;
using Model = SimSDK.Models;

namespace SimSDK.Converters
{
    public static class MessageResponseConverter
    {
        // Convert Model.MessageResponse → Rpc.MessageResponse
        public static Rpc.MessageResponse ToProto(Model.MessageResponse model) => new()
        {
            OutboundMessages = { model.OutboundMessages?.Select(m => SimMessageConverter.ToProto(m)).ToList() ?? new() }
        };

        // Convert Rpc.MessageResponse → Model.MessageResponse
        public static Model.MessageResponse FromProto(Rpc.MessageResponse proto) => new()
        {
            OutboundMessages = proto.OutboundMessages?.Select(m => SimMessageConverter.FromProto(m)).ToList() ?? new()
        };
    }
}