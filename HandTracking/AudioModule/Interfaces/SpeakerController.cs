﻿using System;
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
        ///     Method that returns the closest speaker flag to the hand position.
        /// </summary>
        /// <param name="handLocation"></param>
        public abstract string GetClosest(PXCMPoint3DF32 handLocation);

        /// <summary>
        /// Method which ends the current playback.
        /// </summary>
        public abstract void StopPlayback();        
       

        /// <summary>
        ///     Method that returns the distance between two points in 3D space. The two points must
        ///     be measured in the same units. (e.g. either meters or millimeters)
        /// </summary>
        /// <param name="point1">First point</param>
        /// <param name="point2">Second point</param>
        /// <returns>The distance in the unit of measurement between the two points in cm (assumming points are measured in mm)</returns>
        public static double GetDistance(PXCMPoint3DF32 point1, PXCMPoint3DF32 point2)
        {
            //TODO: there is a gap between centre of hand and centre of marker - aprox 3cm
            return
                Math.Sqrt(Math.Pow(point1.x - point2.x, 2) + Math.Pow(point1.y - point2.y, 2) +
                          Math.Pow(point1.z - point2.z, 2))/10 - Offset;
        }

        #region default vars

        private const int Offset = 2;

        public AudioSettingsImpl AudioSettings { get; protected set; }

        //initial volume

        #endregion
    }
}