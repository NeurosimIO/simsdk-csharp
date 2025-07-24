using System.Collections.Generic;
using SimSDK.Models;

namespace SimSDK.Interfaces
{
    public interface IPlugin
    {
        Manifest GetManifest();
        void CreateComponentInstance(CreateComponentRequest request);
        void DestroyComponentInstance(string componentId);
        List<SimMessage> HandleMessage(SimMessage message);
    }
}