namespace CoreModule.Interfaces
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

        /// <summary>
        /// Method that sets the current participant of the experiment.
        /// </summary>
        /// <param name="participant"></param>
        void SetParticipant(Participant participant);
    }
}