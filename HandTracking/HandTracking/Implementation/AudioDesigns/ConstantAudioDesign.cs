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
    public class ConstantAudioDesign : AudioDesign
    {

        //current file
        #region audiodesign vars

        public override AudioDesignType Type { get; set; }
        public override string File { get; set; }
        private string _filePath = "Sounds\\peakGeiger.wav";
        private Speaker _speaker;

        #endregion

       /* /// <summary>
        /// Sets the audio file to be played.
        /// </summary>
        /// <param name="file"></param>
        public void SetFile(string file)
        {
            throw new NotImplementedException();
        }*/

        /// <summary>
        /// Plays the file through the speaker.
        /// </summary>
        public override void Play(double distance, float volume)
        {
            if(_speaker == null)
                throw new NullReferenceException("Speaker cannot be null.");

            if (volume < 0 || volume > 1)
                throw new ArgumentOutOfRangeException(nameof(volume) + " must be between 0 and 1, floating point.");

            //play file
            _speaker.Play(_filePath, volume);
        }

        /// <summary>
        /// Sets the speaker for the AudioDesign
        /// </summary>
        /// <param name="speaker"></param>
        public override void SetSpeaker(Speaker speaker)
        {
            if (speaker == null)
                throw new NullReferenceException("Speaker cannot be null.");
            _speaker = speaker;

            //set constant design
            _speaker.SetConstant(true);
        }

        /// <summary>
        /// Method that stops playback for current speaker.
        /// </summary>
        public override void StopPlayback()
        {
            _speaker?.StopPlayback();
        }
    }
}
