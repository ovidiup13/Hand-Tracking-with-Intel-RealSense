using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandTracking.Implementation.AudioController;
using HandTracking.Interfaces.AudioController;
using Un4seen.Bass;

namespace HandTracking.Implementation.AudioDesigns
{
    public class ConstantAudioDesign : IAudioDesign
    {

        //current file
        #region audiodesign vars

        public AudioDesignType Type { get; set; }
        private string _filePath = "Sounds\\peakGeiger.wav";
        private Speaker _speaker;

        #endregion

        /// <summary>
        /// Sets the audio file to be played.
        /// </summary>
        /// <param name="file"></param>
        public void SetFile(string file)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Plays the file through the speaker.
        /// </summary>
        public void Play(double distance)
        {
            _speaker.Play(_filePath);
        }

        /// <summary>
        /// Sets the speaker for the AudioDesign
        /// </summary>
        /// <param name="speaker"></param>
        public void SetSpeaker(Speaker speaker)
        {
            _speaker = speaker;
        }
    }
}
