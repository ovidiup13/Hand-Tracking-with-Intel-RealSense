using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary1.Interfaces.AudioController.Designs;
using Un4seen.Bass;

namespace HandTracking.Implementation.AudioDesigns.Geiger
{
    class GeigerCoalescent : GeigerDesign, ICoalescentDesign
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="distance"></param>
        public override void SetDistance(double distance)
        {
            var interval = GetInterval(distance);

            //ignore interval if the same
            if (interval == CurrentInterval)
                return;

            CurrentInterval = interval;
            Delay = CurrentInterval/2;

            PlayBoth();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Play()
        {     
            PlayBoth();
        }

        /// <summary>
        /// 
        /// </summary>
        public void PlayBoth()
        {
            //stop playback, if currently running
            StopPlayback();

            base.Play();
            //create stream
            Stream2 = Bass.BASS_StreamCreateFile(File2, 0L, 0L, WristSpeaker.GetFlag());

            //check stream
            if (Stream2 == 0)
                throw new AudioException("Stream error. Stream cannot be zero. ERROR: " + Bass.BASS_ErrorGetCode());

            Timer = new Timer(obj => { Speaker.Play(Stream); }, null, 50, CurrentInterval);
            Timer2 = new Timer(obj => { WristSpeaker.Play(Stream2); }, null, Delay, CurrentInterval);
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

        #region vars

        protected int Stream2;
        protected Timer Timer2;
        protected int Delay = 450;

        protected string File2 = "Sounds\\Pluck\\obj5p.wav"; //G4

        #endregion
    }
}
