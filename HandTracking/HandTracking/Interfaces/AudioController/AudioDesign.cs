
using System;
using Un4seen.Bass;

namespace HandTracking.Interfaces.AudioController
{
    public enum AudioDesignType
    {
        Individual,
        Coalescent
    };

    public abstract class AudioDesign
    {

        public abstract AudioDesignType Type { get; set; }

        public abstract string File { get; set; }

        /// <summary>
        /// Method that plays audio feedback from referenced speakers.
        /// </summary>
        public abstract void Play(double distance, float volume);

        /// <summary>
        /// Method that sets the next speaker of the Audio Design
        /// </summary>
        public abstract void SetSpeaker(Speaker speaker);

        /// <summary>
        /// Method that stops playback for current speakers.
        /// </summary>
        public abstract void StopPlayback();

    }
}
