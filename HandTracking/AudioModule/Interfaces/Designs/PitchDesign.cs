using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using AudioModule.Interfaces.Designs.Types;

namespace AudioModule.Interfaces.Designs
{
    public abstract class PitchDesign : AudioDesign
    {
        protected PitchDesign(List<string> files)
        {
            CheckFiles(files);
            _files = files;
            CurrentFile = _files[0];
            AudioDesignDesignType = DesignType.Pitch;
        }

        protected PitchDesign()
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
            CurrentFile = _files[0];
            AudioDesignDesignType = DesignType.Pitch;
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

        public override void Play()
        {
            //check speaker
            if (Speaker == null)
            {
                throw new NullReferenceException("Speaker cannot be null.");
            }

            //get default file to play - lowest note
            CurrentFile = GetFile(50);
        }

        public override void StopPlayback()
        {
            Timer?.Dispose();
//            if (Stream != 0)
//            {
//                Speaker.StopPlayback(Stream);
//                Stream = 0;
//            }
        }

        /// <summary>
        ///     Method that returns the file to be played for the current distance.
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        protected string GetFile(double distance)
        {
            if (distance > 40)
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

            if (distance > 7.5 && distance < 15)
            {
                return _files[6];
            }

            if (distance < 7.5)
            {
                return _files[7];
            }

            return _files[0];
        }

        /// <summary>
        ///     Method that sets the distance between hand and target speaker. It selects the appropriate
        ///     file to be played according to the distance and calls the Play method.
        /// </summary>
        /// <param name="distance"></param>
        public override void SetDistance(double distance)
        {
            //get file
            var file = GetFile(distance);

            //if we have the same file, then don't need to change feedback
            if (CurrentFile != null && CurrentFile == file)
            {
                //                Console.WriteLine(@"Skipped current file: " + _currentFile);
                return;
            }

            //otherwise change the file
            CurrentFile = file;
        }

        /// <summary>
        ///     Method that returns the string representation of the audio design.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "PIT_DYNA";
        }

        #region vars

        protected Timer Timer;
        private readonly List<string> _files;
        protected string CurrentFile;
        protected int Stream;

        protected static readonly int Rate = 200;

        #endregion
    }
}