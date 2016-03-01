﻿using System;
using AudioModule.Interfaces;
using MarkerTracking.Implementation;
using Un4seen.Bass;

namespace AudioModule.Implementation.AudioController
{
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
    }
}