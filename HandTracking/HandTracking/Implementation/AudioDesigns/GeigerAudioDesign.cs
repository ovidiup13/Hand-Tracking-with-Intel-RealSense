using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using HandTracking.Interfaces.AudioController;
using Un4seen.Bass;

namespace HandTracking.Implementation.AudioDesigns
{
    public class GeigerAudioDesign : AudioDesign
    {
        /// <summary>
        /// Constructor that initializes a new geiger audio design with default sounds.
        /// </summary>
        public GeigerAudioDesign()
        {
            _files = new List<string>() {"Sounds/Pluck/obj1p.wav", "Sounds/Pluck/obj2p.wav", "Sounds/Pluck/obj3p.wav",
                              "Sounds/Pluck/obj4p.wav", "Sounds/Pluck/obj5p.wav", "Sounds/Pluck/obj6p.wav",
                                "Sounds/Pluck/obj7p.wav","Sounds/Pluck/obj8p.wav"};

            CheckFiles(_files);
        }

        /// <summary>
        /// Constructor that initializes a new geiger design with custom values.
        /// </summary>
        /// <param name="files"></param>
        public GeigerAudioDesign(List<string> files)
        {
            if(_files.Count < 8)
                throw new AudioException("Geiger counter must contain at least 8 audio files.");

            CheckFiles(files);
            _files = files;
        }


        public override void Play(float volume)
        {
            //check speaker
            if (Speaker == null)
            {
                throw new NullReferenceException("Speaker cannot be null.");
            }
            
            //check volume
            if (volume < 0 || volume > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(volume) + " must be between 0 and 1, floating point.");
            }
            _volume = volume;

            //get file to be played
            var file = GetFile(_distance);
            _currentFile = file;

            //play file

            Play(file, _volume);
        }

        /// <summary>
        /// Method that returns the file to be played for the current distance.
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

            if ( distance > 10 && distance < 15)
            {
                return _files[6];
            }

            if (distance < 10)
            {
                return _files[7];
            }

            return _files[0];
        }

        private void Play(string file, float volume)
        {
            //stop the playback and change to other file
            StopPlayback();

            //check file
            if (!File.Exists(file))
                throw new AudioException("File does not exist: " + Path.GetFullPath(file));

            //create stream
            _stream = Bass.BASS_StreamCreateFile(file, 0L, 0L, Speaker.GetFlag());

            //check stream
            if (_stream == 0)
                throw new AudioException("Stream error. Stream cannot be zero. ERROR: " + Bass.BASS_ErrorGetCode());

            //set stream volume
            if (!Bass.BASS_ChannelSetAttribute(_stream, BASSAttribute.BASS_ATTRIB_VOL, volume))
                throw new AudioException("Cannot set volume to stream. ERROR: " + Bass.BASS_ErrorGetCode());

            //play file
            _timer = new Timer(obj => { Speaker.Play(_stream); }, null, 20, 200);
            Console.WriteLine(_timer.ToString());
        }

        /// <summary>
        /// Method that checks if all files exist.
        /// </summary>
        /// <param name="files">List of files passed as argument to the audio design.</param>
        private static void CheckFiles(List<string> files)
        {
            foreach (var file in files.Where(file => !File.Exists(file)))
            {
                throw new AudioException("File does not exist: " + file);
            }
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

        public override void SetDistance(double distance)
        {
            _distance = distance;

            var file = GetFile(distance);
            //if we have the same file, then don't need to change feedback
            if (_currentFile != null && _currentFile == file)
            {
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
        private readonly List<string> _files;
        private string _currentFile;
        private int _stream;
        private double _distance = -1;

        #endregion
    }
}