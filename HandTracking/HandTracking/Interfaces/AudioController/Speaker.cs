using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;

namespace HandTracking.Interfaces.AudioController
{
    public abstract class Speaker
    {
        protected Implementation.AudioController.SpeakerSettingsImpl Settings;
        protected bool Repeat;
        protected int Stream;
        protected float Volume;

        private object lock1 = new object();

        /// <summary>
        /// Method that plays a sound through the Speaker instance.
        /// </summary>
        /// <param name="soundPath">Path to the wav file</param>
        /// <param name="volume">Volume of channel</param>
        public abstract void Play(string soundPath, float volume);

        /// <summary>
        /// Method that sets the current speaker to play continously.
        /// </summary>
        /// <param name="flag"></param>
        public void SetConstant(bool flag)
        {
            Repeat = flag;
        }

        /// <summary>
        /// Method that returns a boolean indicating whether the current speaker is playing a file.
        /// </summary>
        /// <returns></returns>
        private bool IsPlaying()
        {
            return Bass.BASS_ChannelIsActive(Stream) == BASSActive.BASS_ACTIVE_PLAYING;
        }

        /// <summary>
        /// Method that stops the current playback. 
        /// </summary>
        public void StopPlayback()
        {
            //if the stream is not defined, then ignore
            if (Stream == 0)
                return;

            /*
            When looping is enabled, there is a small period of time between sample stop and 
            restart when the channel is set as stopped. This loop is considered a 'hack', to wait
            for that period of time to end, before stopping playback.
            */
//            while (!IsPlaying())
//            {
//                Console.WriteLine("Its not playing...");
//            }
            
            //stop channel playback
            Console.WriteLine(@"Stopping playback...: " + Bass.BASS_ChannelStop(Stream));
        }

    }
}
