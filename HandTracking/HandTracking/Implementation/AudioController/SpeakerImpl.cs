using System;
using HandTracking.Interfaces.AudioController;
using Un4seen.Bass;

namespace HandTracking.Implementation.AudioController
{
    internal class SpeakerImpl : Speaker
    {
        /// <summary>
        /// Constructor for speaker implementation.
        /// </summary>
        /// <param name="flag"></param>
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

            Stream = Repeat ? Bass.BASS_StreamCreateFile(soundPath, 0L, 0L, SpeakerFlag | BASSFlag.BASS_SAMPLE_LOOP) 
                : Bass.BASS_StreamCreateFile(soundPath, 0L, 0L, SpeakerFlag);

            if (!Bass.BASS_ChannelSetAttribute(Stream, BASSAttribute.BASS_ATTRIB_VOL, volume))
            {
                Console.WriteLine(@"[ERROR} Cannot set volume of speaker.");
                return;
            };

            if (Stream != 0)
            {

                // play the stream channel, restart 
                Bass.BASS_ChannelPlay(Stream, true);
            }
            else
            {
                //TODO: throw exception when error is detected
                // error creating the stream 
                Console.WriteLine(@"Stream error: {0}", Bass.BASS_ErrorGetCode());
            }
        }
    }
}