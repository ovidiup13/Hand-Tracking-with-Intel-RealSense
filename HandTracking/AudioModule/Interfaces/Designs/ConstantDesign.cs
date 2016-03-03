using System;
using System.IO;
using System.Threading;
using Un4seen.Bass;

namespace AudioModule.Interfaces.Designs
{
    public abstract class ConstantDesign : AudioDesign
    {
        /// <summary>
        ///     Initializes a new constant design.
        /// </summary>
        protected ConstantDesign()
        {
            File = "Sounds\\Pluck\\obj8p.wav";
            CheckFile(File);
            AudioDesignDesignType = DesignType.Constant;
        }

        /// <summary>
        ///     Constructor with custom parameter.
        /// </summary>
        /// <param name="filePath"></param>
        protected ConstantDesign(string filePath)
        {
            File = filePath;
            CheckFile(File);
            AudioDesignDesignType = DesignType.Constant;
        }

        /// <summary>
        ///     Method that checks if the file exists.
        /// </summary>
        /// <param name="file"></param>
        private void CheckFile(string file)
        {
            if (!System.IO.File.Exists(file))
            {
                throw new AudioException("File does not exist: " + File);
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
            if (!System.IO.File.Exists(File))
                throw new AudioException("File does not exist: " + Path.GetFullPath(File));

            //create stream
            Stream = Bass.BASS_StreamCreateFile(File, 0L, 0L, Speaker.GetFlag());

            //check stream
            if (Stream == 0)
                throw new AudioException("Stream error. Stream cannot be zero. ERROR: " + Bass.BASS_ErrorGetCode());
        }


        /// <summary>
        ///     Method that stops playback for current speaker.
        /// </summary>
        public override void StopPlayback()
        {
            Timer?.Dispose();
            Timer = null;
            if (Stream != 0)
            {
                Speaker.StopPlayback(Stream);
                Stream = 0;
            }
        }

        /// <summary>
        ///     Method that returns the string representation of the audio design.
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

        protected readonly string File;
        protected Timer Timer;
        protected int Stream;
        protected readonly int _interval = 200;

        #endregion
    }
}