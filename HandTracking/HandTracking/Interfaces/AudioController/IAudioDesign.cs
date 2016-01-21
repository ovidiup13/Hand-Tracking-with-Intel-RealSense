
using Un4seen.Bass;

namespace HandTracking.Interfaces.AudioController
{
    public enum AudioDesignType
    {
        Individual,
        Coalescent
    };

    public interface IAudioDesign
    {
        AudioDesignType Type { get; set; }

        /// <summary>
        /// Method that sets file for audio design.
        /// </summary>
        /// <param name="file"></param>
        void SetFile(string file);

        /// <summary>
        /// Method that plays audio feedback from referenced speakers.
        /// </summary>
        void Play(double distance);

        /// <summary>
        /// Method that sets the next speaker of the Audio Design
        /// </summary>
        void SetSpeaker(Speaker speaker);
    }
}
