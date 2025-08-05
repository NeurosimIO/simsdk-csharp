using System.Collections.Generic;
using Model = SimSDK.Models;

namespace SimSDK.Interfaces
{
    public interface IStreamHandler
    {
        List<Model.SimMessage> OnSimMessage(Model.SimMessage message);
        void OnInit(Model.PluginInit init); // Model type, not proto
        void OnShutdown(string reason);
    }
}