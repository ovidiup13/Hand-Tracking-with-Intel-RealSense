using System;
using System.IO;
using System.Threading;
using HandTracking.Interfaces.AudioController;
using Un4seen.Bass;

namespace HandTracking.Implementation.AudioDesigns
{
    public class ConstantAudioDesign : AudioDesign
    {
        public ConstantAudioDesign()
        {
            _file = "Sounds\\Pluck\\obj8p.wav";
        }

        public ConstantAudioDesign(string filePath)
        {
            _file = filePath;
        }

        /// <summary>
        ///     Plays the file through the speaker.
        /// </summary>
        public override void Play(double distance, float volume)
        {
            //check speaker
            if (Speaker == null)
                throw new NullReferenceException("Speaker cannot be null.");

            //check volume
            if (volume < 0 || volume > 1)
                throw new ArgumentOutOfRangeException(nameof(volume) + " must be between 0 and 1, floating point.");

            //check file
            if (!File.Exists(_file))
                throw new AudioException("File does not exist: " + Path.GetFullPath(_file));

            //create stream
            _stream = Bass.BASS_StreamCreateFile(_file, 0L, 0L, Speaker.GetFlag());

            //check stream
            if (_stream == 0)
                throw new AudioException("Stream error. Stream cannot be zero. ERROR: " + Bass.BASS_ErrorGetCode());

            //set stream volume
            if (!Bass.BASS_ChannelSetAttribute(_stream, BASSAttribute.BASS_ATTRIB_VOL, volume))
                throw new AudioException("Cannot set volume to stream. ERROR: " + Bass.BASS_ErrorGetCode());

            //play file
            _timer = new Timer(obj => { Speaker.Play(_stream); }, null, 200, _interval);
        }

        /// <summary>
        ///     Method that stops playback for current speaker.
        /// </summary>
        public override void StopPlayback()
        {
            _timer?.Dispose();
            Speaker?.StopPlayback(_stream);
        }

        /// <summary>
        /// Method that sets the current distance between speaker and hand.
        /// Not used in constant design.
        /// </summary>
        public override void SetDistance()
        {}

        #region audiodesign vars

        private string _file;
        private Timer _timer;
        private int _stream;
        private int _interval = 200;

        #endregion
    }
}