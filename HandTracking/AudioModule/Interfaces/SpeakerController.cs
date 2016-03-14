using System;
using AudioModule.Implementation.AudioController;
using Un4seen.Bass;

namespace AudioModule.Interfaces
{
    public abstract class SpeakerController
    {
        /// <summary>
        ///     Method that returns a next random speaker.
        /// </summary>
        /// <returns>Next speaker for current condition</returns>
        public abstract void NextSpeaker();

        /// <summary>
        ///     Method that plays a confirmation sound indication for end of current trial speaker.
        /// </summary>
        public abstract void PlayConfirm();

        /// <summary>
        ///     Set the Audio design for the current condition.
        /// </summary>
        /// <param name="audioDesign"></param>
        public abstract void SetAudioDesign(AudioDesign audioDesign);

        /// <summary>
        ///     Method that plays the according sound specified by the Audio Design.
        /// </summary>
        public abstract void PlaySounds();

        /// <summary>
        ///     Method that returns the number of speakers registered with the controller.
        /// </summary>
        /// <returns></returns>
        public abstract int GetNumberOfSpeakers();

        /// <summary>
        ///     Method that signals to the speaker controller that the current condition has ended.
        ///     It will set the audio design to null and randomize speaker order for a new condition.
        /// </summary>
        /// <param name="flag"></param>
        public abstract void SignalConditionEnded(bool flag);

        /// <summary>
        ///     Method that signals to the speaker controller that the current trial has ended.
        ///     It will randomize speaker order for a new trial.
        /// </summary>
        /// <param name="flag"></param>
        public abstract void SignalTrialEnded(bool flag);

        /// <summary>
        ///     Returns the position of the current target speaker.
        /// </summary>
        /// <returns>position of target speaker.</returns>
        public abstract PXCMPoint3DF32 GetSpeakerPosition();

        /// <summary>
        ///     Method that returns the target speaker id.
        /// </summary>
        /// <returns>speaker id as int</returns>
        public abstract string GetSpeakerId();

        /// <summary>
        ///     Method that sets the current distance between hand and target speaker.
        /// </summary>
        /// <param name="distance"></param>
        public abstract void SetDistance(double distance);

        /// <summary>
        /// Method which ends the current playback.
        /// </summary>
        public abstract void StopPlayback();

        /// <summary>
        /// Method that disposes the BASS libraries.
        /// </summary>
        public abstract void CleanUp();

        #region default 

        public AudioSettingsImpl AudioSettings { get; protected set; }

        //initial volume

        #endregion
    }
}