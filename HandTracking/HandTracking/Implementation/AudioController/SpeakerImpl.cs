using System;
using HandTracking.Interfaces.AudioController;
using Un4seen.Bass;

namespace HandTracking.Implementation.AudioController
{
    internal class SpeakerImpl : Speaker
    {
        public SpeakerImpl(BASSFlag flag)
        {
            SpeakerFlag = flag;
        }

        private BASSFlag SpeakerFlag { get; }

        /// <summary>
        ///     TODO: might have to create the stream in audio design and pass it here in order to play it
        ///     - not sure how continuously playing the signal through a separate thread would work
        ///     - it might not be a good idea to create a new stream every time
        /// </summary>
        /// <param name="soundPath"></param>
        /// <param name="volume"></param>
        public override void Play(string soundPath, float volume)
        {
            //create a new stream and play it
            var stream = Bass.BASS_StreamCreateFile(soundPath, 0L, 0L, SpeakerFlag);
            if (stream != 0 && Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, volume))
            {
                // play the stream channel 
                Bass.BASS_ChannelPlay(stream, false);
            }
            else
            {
                // error creating the stream 
                Console.WriteLine(@"Stream error: {0}", Bass.BASS_ErrorGetCode());
            }
            Bass.BASS_ChannelPlay(stream, false);
        }
    }
}