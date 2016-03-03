using System;
using System.Threading;
using Un4seen.Bass;

namespace AudioModule.Interfaces.Designs
{
    public abstract class GeigerDesign : AudioDesign
    {
        /// <summary>
        ///     Constructor that initializes a new geiger audio design with default sounds.
        /// </summary>
        protected GeigerDesign()
        {
            File = "Sounds/Pluck/obj8p.wav";

            CheckFile(File);

            AudioDesignDesignType = DesignType.Geiger;
        }

        /// <summary>
        ///     Constructor that initializes a new geiger design with custom values.
        /// </summary>
        /// <param name="file"></param>
        protected GeigerDesign(string file)
        {
            CheckFile(file);
            File = file;
            AudioDesignDesignType = DesignType.Geiger;
        }    

        /// <summary>
        ///     Method that checks if all files exist.
        /// </summary>
        /// <param name="file">File path passed as argument to the audio design.</param>
        private static void CheckFile(string file)
        {
            if (!System.IO.File.Exists(file))
                throw new AudioException("File does not exist: " + file);
        }

        /// <summary>
        ///     Method that stops the current playback.
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

        public override void Play()
        {
            //check speaker
            if (Speaker == null)
                throw new NullReferenceException("Speaker cannot be null.");

            //create stream
            Stream = Bass.BASS_StreamCreateFile(File, 0L, 0L, Speaker.GetFlag());

            //check stream
            if (Stream == 0)
                throw new AudioException("Stream error. Stream cannot be zero. ERROR: " + Bass.BASS_ErrorGetCode());

        }

        /// <summary>
        /// Method that returns the string representation of the audio design.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "GEI_DYNA";
        }

        protected int GetInterval(double distance)
        {
            if (distance > 45)
            {
                return 900;
            }

            if (distance > 40 && distance < 45)
            {
                return 800;
            }

            if (distance > 35 && distance < 40)
            {
                return 700;
            }

            if (distance > 30 && distance < 35)
            {
                return 600;
            }

            if (distance > 25 && distance < 30)
            {
                return 500;
            }

            if (distance > 20 && distance < 25)
            {
                return 400;
            }

            if (distance > 15 && distance < 20)
            {
                return 300;
            }

            if (distance > 7.5 && distance < 15)
            {
                return 200;
            }

            return 100;
        }

        #region vars

        protected Timer Timer;
        protected int CurrentInterval = 900;
        protected readonly string File;
        protected int Stream;

        #endregion
    }
}