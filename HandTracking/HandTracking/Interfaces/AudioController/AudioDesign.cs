﻿using System;
using Un4seen.Bass;

namespace HandTracking.Interfaces.AudioController
{
    public abstract class AudioDesign
    {
        /// <summary>
        ///     Method that plays audio feedback from referenced speakers.
        /// </summary>
        public abstract void Play(double distance, float volume);

        /// <summary>
        ///     Sets the speaker for the AudioDesign
        /// </summary>
        /// <param name="speaker"></param>
        public void SetSpeaker(Speaker speaker)
        {
            if (speaker == null)
                throw new NullReferenceException("Speaker cannot be null.");
            Speaker = speaker;
        }

        /// <summary>
        ///     Method that stops playback for current speakers.
        /// </summary>
        public abstract void StopPlayback();

        /// <summary>
        ///     Method that returns the speaker position.
        /// </summary>
        /// <returns>position of speaker in 3D</returns>
        public PXCMPoint3DF32 GetSpeakerPosition()
        {
            return Speaker.GetPosition();
        }

        /// <summary>
        ///     Method that returns the id of the target speaker.
        /// </summary>
        /// <returns></returns>
        public int GetSpeakerId()
        {
            return Speaker.GetSpeakerId();
        }

        /// <summary>
        ///     Method that sets the volume of the speaker
        /// </summary>
        public void SetVolume(float volume)
        {
            
        }

        /// <summary>
        /// Method that sets the distance between hand and target speaker.
        /// </summary>
        public abstract void SetDistance();

        #region vars

        protected Speaker Speaker;

        #endregion
    }
}