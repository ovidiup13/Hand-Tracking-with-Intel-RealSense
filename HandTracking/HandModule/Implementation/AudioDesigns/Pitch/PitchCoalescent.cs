using System;
using System.Collections.Generic;
using System.Threading;
using ClassLibrary1.Interfaces.AudioController.Designs;
using Un4seen.Bass;

namespace HandTracking.Implementation.AudioDesigns.Pitch
{
    public class PitchCoalescent : PitchDesign, ICoalescentDesign
    {
        public PitchCoalescent()
        {
        }

        public PitchCoalescent(List<string> files) : base(files)
        {
        }

        public override void Play()
        {
            base.Play();
            PlayBoth();
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
                return;
            }

            //otherwise change the file
            CurrentFile = file;

            PlayBoth();
        }

        /// <summary>
        /// Method that stops the current playback.
        /// </summary>
        public override void StopPlayback()
        {
            base.StopPlayback();
            Timer2?.Dispose();
            Timer2 = null;
            if (Stream2 != 0)
            {
                WristSpeaker?.StopPlayback(Stream2);
                Stream2 = 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return base.ToString() + "_BOT";
        }

        /// <summary>
        /// 
        /// </summary>
        public void PlayBoth()
        {
            //stop the playback and change to other file
            StopPlayback();

            //create stream
            Stream2 = Bass.BASS_StreamCreateFile(CurrentFile, 0L, 0L, WristSpeaker.GetFlag());
            Stream = Bass.BASS_StreamCreateFile(TargetFile, 0L, 0L, Speaker.GetFlag());
            
            //check stream
            if (Stream == 0)
                throw new AudioException("Stream error. Stream cannot be zero. ERROR: " + Bass.BASS_ErrorGetCode());

            //play file
            Timer = new Timer(obj => { Speaker.Play(Stream); }, null, 0, Rate);
            Timer2 = new Timer(obj => { WristSpeaker.Play(Stream2); }, null, Delay, Rate);
        }

        #region vars

        protected int Stream2;
        protected Timer Timer2;

        private static readonly int Delay = Rate/2;

        private static readonly string TargetFile = "Sounds\\Pluck\\obj8p.wav"; 

        #endregion
    }
}