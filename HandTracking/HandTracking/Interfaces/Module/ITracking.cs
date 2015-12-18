using System.Threading;
using HandTracking.Interfaces.Settings;

namespace HandTracking.Interfaces.Module
{
    public interface ITracking
    {

        void StartProcessing();

        void StopProcessing();

        void PauseProcessing();

        IData GetData();
    }
}