using System;
using System.IO;
using System.Threading;
using HandTracking.Interfaces.AudioController;
using Un4seen.Bass;

namespace HandTracking.Implementation.AudioDesigns
{
    public class GeigerAudioDesign : AudioDesign
    {
        /// <summary>
        ///     Constructor that initializes a new geiger audio design with default sounds.
        /// </summary>
        public GeigerAudioDesign()
        {
            _file = "Sounds/Pluck/obj8p.wav";

            CheckFile(_file);
        }

        /// <summary>
        ///     Constructor that initializes a new geiger design with custom values.
        /// </summary>
        /// <param name="file"></param>
        public GeigerAudioDesign(string file)
        {
            CheckFile(file);
            _file = file;
        }


        public override void Play()
        {
            //check speaker
            if (Speaker == null)
            {
                throw new NullReferenceException("Speaker cannot be null.");
            }

            //create stream
            _stream = Bass.BASS_StreamCreateFile(_file, 0L, 0L, Speaker.GetFlag());

            //check stream
            if (_stream == 0)
                throw new AudioException("Stream error. Stream cannot be zero. ERROR: " + Bass.BASS_ErrorGetCode());

            //play file
            _timer = new Timer(obj => { Speaker.Play(_stream); }, null, 50, _currentInterval);
        }

        /// <summary>
        ///     Method that checks if all files exist.
        /// </summary>
        /// <param name="file">File path passed as argument to the audio design.</param>
        private static void CheckFile(string file)
        {
            if (!File.Exists(file))
                throw new AudioException("File does not exist: " + file);
        }

        /// <summary>
        ///     Method that stops the current playback.
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
        ///     Method that sets the distance between hand and target speaker. It selects the appropriate
        ///     file to be played according to the distance and calls the Play method.
        /// </summary>
        /// <param name="distance"></param>
        public override void SetDistance(double distance)
        {
            //get interval based on distance
            var interval = GetInterval(distance);

            //if we have the same interval, we don't update the timer
            if (_currentInterval == interval)
                return;

            _currentInterval = interval;

            //update timer
            _timer?.Change(170, _currentInterval);
        }

        private int GetInterval(double distance)
        {
            if (distance > 40 || distance < 0)
            {
                return 900;
            }

            if (distance > 35 && distance < 40)
            {
                return 800;
            }

            if (distance > 30 && distance < 35)
            {
                return 700;
            }

            if (distance > 25 && distance < 30)
            {
                return 600;
            }

            if (distance > 20 && distance < 25)
            {
                return 500;
            }

            if (distance > 15 && distance < 20)
            {
                return 400;
            }

            if (distance > 10 && distance < 15)
            {
                return 300;
            }

            if (distance < 10)
            {
                return 200;
            }

            return 900;
        }

        #region vars

        private Timer _timer;
        private int _currentInterval = 900;
        private readonly string _file;
        private int _stream;

        #endregion
    }
}