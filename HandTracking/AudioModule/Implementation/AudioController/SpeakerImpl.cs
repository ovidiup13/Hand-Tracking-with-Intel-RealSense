using System;
using AudioModule.Interfaces;
using CameraModule.Implementation.MarkerTracking;
using Un4seen.Bass;

namespace AudioModule.Implementation.AudioController
{
    //TODO: copy all speaker methods in here
    public class SpeakerImpl : Speaker
    {
        /// <summary>
        ///     Constructor for speaker implementation.
        /// TODO: set protection level to protected internal
        /// </summary>
        /// <param name="flag"></param>
        public SpeakerImpl(BASSFlag flag)
        {
            SpeakerFlag = flag;
        }

        /// <summary>
        ///     Constructor for speaker implementation.
        /// TODO: set protection level to protected internal
        /// </summary>
        /// <param name="marker">marker object assigned to speaker</param>
        /// <param name="flag">speaker flag</param>
        public SpeakerImpl(Marker marker, BASSFlag flag)
        {
            Marker = marker;
            SpeakerFlag = flag;

            Console.WriteLine(@"Speaker initialized with id: " + Marker.Id + @", speaker: " + SpeakerFlag);
        }

        /// <summary>
        ///     Method that stops the current playback.
        /// </summary>
        public new void StopPlayback(int stream)
        {
            //if the stream is not defined, then ignore
            if (stream == 0)
                throw new AudioException("Stream error. Stream cannot be zero. Error: " + Bass.BASS_ErrorGetCode());

            //wait until finished
            //            while (IsPlaying(stream)) { }

            if (!Bass.BASS_ChannelStop(stream))
                throw new AudioException("Cannot stop playback. Error: " + Bass.BASS_ErrorGetCode());

            //free stream
            Bass.BASS_StreamFree(stream);
        }
    }
}