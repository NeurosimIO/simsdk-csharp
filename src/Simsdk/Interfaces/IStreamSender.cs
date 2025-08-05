using System.Threading.Tasks;
using Model = SimSDK.Models;

namespace SimSDK.Interfaces
{
    public interface IStreamSender
    {
        Task Send(Model.SimMessage message);
        string ComponentId();
    }
}