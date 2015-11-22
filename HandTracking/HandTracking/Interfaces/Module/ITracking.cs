using System.Threading;
using HandTracking.Interfaces.Settings;

namespace HandTracking.Interfaces.Module
{
    public interface ITracking
    {
        ISettings Settings { get; set; }

        void StartProcessing();

        void StopProcessing();

        void PauseProcessing();

        IData GetHandData();
    }
}