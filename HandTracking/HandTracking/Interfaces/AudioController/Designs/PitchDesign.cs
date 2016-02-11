using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Un4seen.Bass;

namespace HandTracking.Interfaces.AudioController.Designs
{
    public abstract class PitchDesign : AudioDesign
    {
        public PitchDesign(List<string> files)
        {
            CheckFiles(files);
            _files = files;
            _currentFile = _files[0];
        }

        public PitchDesign()
        {
            _files = new List<string>
            {
                "Sounds/Pluck/obj1p.wav",
                "Sounds/Pluck/obj2p.wav",
                "Sounds/Pluck/obj3p.wav",
                "Sounds/Pluck/obj4p.wav",
                "Sounds/Pluck/obj5p.wav",
                "Sounds/Pluck/obj6p.wav",
                "Sounds/Pluck/obj7p.wav",
                "Sounds/Pluck/obj8p.wav"
            };

            CheckFiles(_files);
            _currentFile = _files[0];
        }

        /// <summary>
        ///     Method that checks if all files exist.
        /// </summary>
        /// <param name="files">List of files passed as argument to the audio design.</param>
        private static void CheckFiles(List<string> files)
        {
            if (files.Count < 8)
                throw new AudioException("Dynamic Pitch must contain at least 8 audio files.");

            foreach (var file in files.Where(file => !File.Exists(file)))
            {
                throw new AudioException("File does not exist: " + file);
            }
        }

        /// <summary>
        ///     Method that stops the current playback and starts a new timer with the selected file and
        ///     the specified volume.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="volume"></param>
        private void Play(string file, float volume)
        {
            //stop the playback and change to other file
            StopPlayback();

            //create stream
            _stream = Bass.BASS_StreamCreateFile(_currentFile, 0L, 0L, Speaker.GetFlag());

            //check stream
            if (_stream == 0)
                throw new AudioException("Stream error. Stream cannot be zero. ERROR: " + Bass.BASS_ErrorGetCode());

            //play file
            _timer = new Timer(obj => { Speaker.Play(_stream); }, null, 0, 200);
            Console.WriteLine(_timer.ToString());
        }

        public override void Play()
        {
            //check speaker
            if (Speaker == null)
            {
                throw new NullReferenceException("Speaker cannot be null.");
            }

            //get file to be played
            var file = GetFile(_distance);
            _currentFile = file;

            //play file
            Play(file, _volume);
        }

        public override void StopPlayback()
        {
            _timer?.Dispose();
            if (_stream != 0)
            {
                Speaker?.StopPlayback(_stream);
                _stream = 0;
            }
        }
       
        /// <summary>
        ///     Method that returns the file to be played for the current distance.
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        private string GetFile(double distance)
        {
            if (distance > 40 || distance < 0)
            {
                return _files[0];
            }

            if (distance > 35 && distance < 40)
            {
                return _files[1];
            }

            if (distance > 30 && distance < 35)
            {
                return _files[2];
            }

            if (distance > 25 && distance < 30)
            {
                return _files[3];
            }

            if (distance > 20 && distance < 25)
            {
                return _files[4];
            }

            if (distance > 15 && distance < 20)
            {
                return _files[5];
            }

            if (distance > 10 && distance < 15)
            {
                return _files[6];
            }

            if (distance < 10)
            {
                return _files[7];
            }

            return _files[0];
        }

        /// <summary>
        /// Method that returns the string representation of the audio design.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "PIT_DYNA_IND";
        }

        /// <summary>
        ///     Method that sets the distance between hand and target speaker. It selects the appropriate
        ///     file to be played according to the distance and calls the Play method.
        /// </summary>
        /// <param name="distance"></param>
        public override void SetDistance(double distance)
        {
            //set distance
            _distance = distance;

            //get file
            var file = GetFile(distance);

            //if we have the same file, then don't need to change feedback
            if (_currentFile != null && _currentFile == file)
            {
//                Console.WriteLine(@"Skipped current file: " + _currentFile);
                return;
            }

            //otherwise change the file
            _currentFile = file;

            //play next sound
            Play(file, _volume);
        }

        #region vars


        private float _volume;
        private Timer _timer;
        private List<string> _files;
        private readonly string _file;
        private string _currentFile;
        private int _stream;
        private double _distance = -1;

        #endregion
    }
}