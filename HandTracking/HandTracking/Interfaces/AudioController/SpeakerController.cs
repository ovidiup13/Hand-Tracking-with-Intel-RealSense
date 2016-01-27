using System;
using System.Collections.Generic;
using Un4seen.Bass;

namespace HandTracking.Interfaces.AudioController
{
    public abstract class SpeakerController
    {

        private static readonly float VolumeGap = 0.05f;

        //initial volume
        protected float Volume = 0.5f;

        /// <summary>
        /// Method that returns a next random speaker.
        /// </summary>
        /// <returns>Next speaker for current condition</returns>
        public abstract void NextSpeaker();

        /// <summary>
        /// Set the Audio design for the current condition.
        /// </summary>
        /// <param name="audioDesign"></param>
        public abstract void SetAudioDesign(AudioDesign audioDesign);

        /// <summary>
        /// Method that plays the according sound specified by the Audio Design.
        /// </summary>
        public abstract void PlaySounds(double distance);

        /// <summary>
        /// Method that returns the number of speakers registered with the controller.
        /// </summary>
        /// <returns></returns>
        public abstract int GetNumberOfSpeakers();

        /// <summary>
        /// Method that signals to the speaker controller that the current condition has ended.
        /// It will set the audio design to null and randomize speaker order for a new condition.
        /// </summary>
        /// <param name="flag"></param>
        public abstract void SignalConditionEnded(bool flag);

        /// <summary>
        /// Method that signals to the speaker controller that the current trial has ended.
        /// It will randomize speaker order for a new trial.
        /// </summary>
        /// <param name="flag"></param>
        public abstract void SignalTrialEnded(bool flag);

        /// <summary>
        /// Method that sets the current volume for all speakers.
        /// </summary>
        /// <param name="value">Floating point value between 0 and 1.</param>
        public void SetVolume(float value)
        {
            if (Volume < 0 || Volume > 1)
                throw new ArgumentOutOfRangeException(nameof(value) + " must be between 0 and 1.");

            Volume = value;
        }

        /// <summary>
        /// Method that increases the volume.
        /// </summary>
        public void IncreaseVolume()
        {
            //check if volume already maximum
            if (Math.Abs(Volume - 1) <= 0)
                return;

            //increase volume
            Volume += VolumeGap;
        }

        /// <summary>
        /// Method that decreases the current volume.
        /// </summary>
        public void DecreaseVolume()
        {
            //if already 0, skip
            if (Math.Abs(Volume) <= 0)
                return;

            //decrease volume
            Volume -= VolumeGap;
        }

        /*/// <summary>
        /// Method that sets speaker data.
        /// </summary>
        /// <param name="speakers"></param>
        public abstract void SetSpeakers(List<Speaker> speakers);

        /// <summary>
        /// Method that retrieves a list of all speakers.
        /// </summary>
        /// <returns>List of all speakers.</returns>
        public abstract List<Speaker> GetAllSpeakers();

        

       

        /// <summary>
        /// Method that sets the volume of speakers.
        /// </summary>
        /// <param name="volume"></param>
        void SetVolume(float volume);

        /// <summary>
        /// Method that signals the speaker controller to stop producing sound.
        /// </summary>
        void StopStreaming();*/
    }
}