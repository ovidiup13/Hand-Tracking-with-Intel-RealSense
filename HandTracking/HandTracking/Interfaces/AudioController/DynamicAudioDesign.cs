using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Un4seen.Bass;

namespace HandTracking.Interfaces.AudioController
{
    internal abstract class DynamicAudioDesign : AudioDesign
    {
        protected void Play(string file, float volume)
        {
            //stop the playback and change to other file
            StopPlayback();

            //check file
            if (!File.Exists(file))
                throw new AudioException("File does not exist: " + Path.GetFullPath(file));

            //create stream
            Stream = Bass.BASS_StreamCreateFile(file, 0L, 0L, Speaker.GetFlag());

            //check stream
            if (Stream == 0)
                throw new AudioException("Stream error. Stream cannot be zero. ERROR: " + Bass.BASS_ErrorGetCode());

            //set stream volume
            if (!Bass.BASS_ChannelSetAttribute(Stream, BASSAttribute.BASS_ATTRIB_VOL, volume))
                throw new AudioException("Cannot set volume to stream. ERROR: " + Bass.BASS_ErrorGetCode());

            //play file
            Timer = new Timer(obj => { Speaker.Play(Stream); }, null, 20, 200);
            Console.WriteLine(Timer.ToString());
        }

        #region vars

        protected int Stream;
        protected Timer Timer;

        #endregion
    }
}