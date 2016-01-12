namespace HandTracking.Interfaces.Core
{
    public interface IExperiment
    {
        /// <summary>
        /// 
        /// </summary>
        void StartExperiment();

        /// <summary>
        /// 
        /// </summary>
        void PauseExperiment();

        /// <summary>
        /// 
        /// </summary>
        void StopExperiment();

        /// <summary>
        /// 
        /// </summary>
        void NextTrial();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool IsStarted();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool IsStopped();
    }
}