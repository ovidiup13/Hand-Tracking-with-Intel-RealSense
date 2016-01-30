using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using HandTracking.Interfaces.AudioController;
using Un4seen.Bass;

namespace HandTracking.Implementation.AudioDesigns
{
    class GeigerAudioDesign : AudioDesign
    {
        /// <summary>
        /// Constructor that initializes a new geiger audio design with default sounds.
        /// </summary>
        public GeigerAudioDesign()
        {
            _files = new List<string>() {"Sounds/Pluck/obj1p.wav", "Sounds/Pluck/obj2p.wav", "Sounds/Pluck/obj3p.wav",
                              "Sounds/Pluck/obj4p.wav", "Sounds/Pluck/obj5p.wav", "Sounds/Pluck/obj6p.wav",
                                "Sounds/Pluck/obj7p.wav","Sounds/Pluck/obj8p.wav"};
        }

        /// <summary>
        /// Constructor that initializes a new geiger design with custom values.
        /// </summary>
        /// <param name="files"></param>
        public GeigerAudioDesign(List<string> files)
        {
            if(_files.Count < 8)
                throw new AudioException("Geiger counter must contain at least 8 audio files.");

            _files = files;
        }


        public override void Play(double distance, float volume)
        {
            //check speaker
            if (Speaker == null)
                throw new NullReferenceException("Speaker cannot be null.");

            //check volume
            if (volume < 0 || volume > 1)
                throw new ArgumentOutOfRangeException(nameof(volume) + " must be between 0 and 1, floating point.");

            //get file to be played
            var file = GetFile(distance);

            //play file
            Play(file, volume);

        }

        /// <summary>
        /// Method that returns the file to be played for the current distance.
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        private string GetFile(double distance)
        {
          
            if (distance > 40)
            {
                return _files[1];
            }

            if (distance > 35 && distance < 40)
            {
                return _files[1];
            }

            if (distance > 30 && distance < 35)
            {
                return _files[1];
            }

            if (distance > 25 && distance < 30)
            {
                return _files[1];
            }

            if (distance > 20 && distance < 25)
            {
                return _files[1];
            }

            if (distance > 15 && distance < 20)
            {
                return _files[1];
            }

            if (distance < 15)
            {
                return _files[7];
            }

            return null;
        }

        private void Play(string file, float volume)
        {
            //dispose timer if already defined
            _timer?.Dispose();

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
            _timer = new Timer(obj => { Speaker.Play(_stream); }, null, 200, 200);
        }

        public override void StopPlayback()
        {
            _timer?.Dispose();
            Speaker.StopPlayback(_stream);
        }

        public override void SetDistance()
        {
            throw new NotImplementedException();
        }

        #region vars

        private Timer _timer;
        private List<string> _files;
        private int _stream;

        #endregion
    }
}