using System.Collections.Generic;
using Model = SimSDK.Models;

namespace SimSDK.Interfaces
{
    public interface IPlugin
    {
        Model.Manifest GetManifest();
        void CreateComponentInstance(Model.CreateComponentRequest request);
        void DestroyComponentInstance(string componentId);
        List<Model.SimMessage> HandleMessage(Model.SimMessage message);
    }

    public interface IPluginWithHandlers : IPlugin
    {
        IStreamHandler GetStreamHandler(); // Will now refer to the separate interface
    }
}