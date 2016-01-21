using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandTracking.Interfaces.AudioController;
using OpenCV.Net;
using Un4seen.Bass;

namespace HandTracking.Implementation.AudioController
{
    class SpeakerImpl: Interfaces.AudioController.Speaker
    {
        protected internal BASSFlag SpeakerFlag { get; set; }

        public SpeakerImpl(BASSFlag flag)
        {
            SpeakerFlag = flag;
        }

        /// <summary>
        /// TODO: might have to create the stream in audio design and pass it here in order to play it
        /// - not sure how continuously playing the signal through a separate thread would work
        /// - it might not be a good idea to create a new stream every time
        /// </summary>
        /// <param name="soundPath"></param>
        public override void Play(string soundPath)
        {
            //create a new stream and play it
            int stream1 = Bass.BASS_StreamCreateFile(soundPath, 0L, 0L, SpeakerFlag);
            Bass.BASS_ChannelPlay(stream1, false);
        }
    }
}
