namespace HandTracking.Interfaces.Core
{
    public interface IExperiment
    {
        void StartExperiment();
        void PauseExperiment();
        void StopExperiment();
        void NextTrial();
        bool IsStarted();
        bool IsStopped();
    }
}