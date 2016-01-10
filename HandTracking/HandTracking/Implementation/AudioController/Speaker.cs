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
    class Speaker: ISpeaker
    {
        private BASSFlag _speaker;
        private string _file = "Sounds\\peakGeiger.wav";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="speaker"></param>
        public Speaker(BASSFlag speaker)
        {
            _speaker = speaker;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Play()
        {
            var stream1 = Bass.BASS_StreamCreateFile("Sounds\\peakGeiger.wav", 0L, 0L, _speaker);
            Bass.BASS_ChannelPlay(stream1, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        public void SetFile(String file)
        {
            _file = file;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsPlaying()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
