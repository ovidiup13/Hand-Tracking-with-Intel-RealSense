
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
    }
}
