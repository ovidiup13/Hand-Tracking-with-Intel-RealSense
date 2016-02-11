using System;
using System.IO;
using System.Threading;
using Un4seen.Bass;

namespace HandTracking.Interfaces.AudioController.Designs
{
    public abstract class ConstantDesign : AudioDesign
    {

        /// <summary>
        ///     Method that checks if the file exists.
        /// </summary>
        /// <param name="file"></param>
        protected void CheckFile(string file)
        {
            if (!File.Exists(file))
            {
                throw new AudioException("File does not exist: " + _file);
            }
        }

        /// <summary>
        ///     Plays the file through the speaker.
        /// </summary>
        public override void Play()
        {
            //check speaker
            if (Speaker == null)
                throw new NullReferenceException("Speaker cannot be null.");

            //check file
            if (!File.Exists(_file))
                throw new AudioException("File does not exist: " + Path.GetFullPath(_file));

            //create stream
            _stream = Bass.BASS_StreamCreateFile(_file, 0L, 0L, Speaker.GetFlag());

            //check stream
            if (_stream == 0)
                throw new AudioException("Stream error. Stream cannot be zero. ERROR: " + Bass.BASS_ErrorGetCode());

        }


        /// <summary>
        ///     Method that stops playback for current speaker.
        /// </summary>
        public override void StopPlayback()
        {
            _timer?.Dispose();
            _timer = null;
            if (_stream != 0)
            {
                Speaker?.StopPlayback(_stream);
                _stream = 0;
            }
        }

        /// <summary>
        /// Method that returns the string representation of the audio design.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "CONST_STAT";
        }

        /// <summary>
        ///     Method that sets the current distance between speaker and hand.
        ///     Not used in constant design.
        /// </summary>
        public override void SetDistance(double distance)
        {
        }

        #region audiodesign vars

        protected string _file;
        protected Timer _timer;
        protected int _stream;
        protected readonly int _interval = 200;

        #endregion
    }
}