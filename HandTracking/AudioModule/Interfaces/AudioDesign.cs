using System;
using AudioModule.Implementation.AudioController;
using Un4seen.Bass;

namespace AudioModule.Interfaces
{
    internal interface ICoalescentDesign
    {
        void PlayBoth();
    }

    internal interface INdividualDesign
    {
        void PlayIndividual();
    }

    public abstract class AudioDesign
    {
        /// <summary>
        ///     Method that plays audio feedback from referenced speakers.
        /// </summary>
        public abstract void Play();

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
        public string GetSpeakerId()
        {
            return Speaker.GetFlag().ToString();
        }

        /// <summary>
        /// Method that returns the string representation of the audio design.
        /// </summary>
        /// <returns></returns>
        public abstract override string ToString();

        /// <summary>
        /// Method that sets the distance between hand and target speaker.
        /// </summary>
        public abstract void SetDistance(double distance);

        public static void SetWristSpeaker(BASSFlag flag)
        {
            WristSpeaker = new SpeakerImpl(flag);
        }

        public static BASSFlag GetWristSpeaker()
        {
            return WristSpeaker.GetFlag();
        }

        #region vars

        protected Speaker Speaker;

        private static BASSFlag _defaultWristSpeaker = BASSFlag.BASS_SPEAKER_REAR2RIGHT; //8

        protected static Speaker WristSpeaker { get; set; } = new SpeakerImpl(flag: _defaultWristSpeaker);
    

        #endregion
    }
}