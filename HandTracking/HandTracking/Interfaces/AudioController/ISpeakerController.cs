using System.Collections.Generic;
using Un4seen.Bass;

namespace HandTracking.Interfaces.AudioController
{
    public interface ISpeakerController
    {

        /// <summary>
        /// Method that sets speaker data.
        /// </summary>
        /// <param name="speakers"></param>
        void SetSpeakers(List<ISpeaker> speakers);

        /// <summary>
        /// Method that retrieves a list of all speakers.
        /// </summary>
        /// <returns>List of all speakers.</returns>
        List<ISpeaker> GetAllSpeakers();

        /// <summary>
        /// Method that returns a next random speaker.
        /// </summary>
        /// <returns>Next speaker for current condition</returns>
        BASSFlag? GetNextSpeaker();

        /// <summary>
        /// Set the Audio design for the current condition.
        /// </summary>
        /// <param name="audioDesign"></param>
        void SetAudioDesign(IAudioDesign audioDesign);

        /// <summary>
        /// 
        /// </summary>
        void PlaySounds();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int GetNumberOfSpeakers();

        /// <summary>
        /// Method that sets the volume of speakers.
        /// </summary>
        /// <param name="volume"></param>
        void SetVolume(float volume);

        /// <summary>
        /// Method that signals the speaker controller to stop producing sound.
        /// </summary>
        void StopStreaming();
    }
}