﻿using System.Threading;
using ClassLibrary1.Interfaces.AudioController.Designs;
using Un4seen.Bass;

namespace HandTracking.Implementation.AudioDesigns.Constant
{
    internal class ConstantCoalescent : ConstantDesign, ICoalescentDesign
    {
        /// <summary>
        ///     Initializes a new constant design.
        /// </summary>
        public ConstantCoalescent()
        {
        }

        /// <summary>
        ///     Constructor with custom parameter.
        /// </summary>
        /// <param name="filePath"></param>
        public ConstantCoalescent(string filePath) : base(filePath)
        {
        }

        public void PlayBoth()
        {
            //create stream
            Stream2 = Bass.BASS_StreamCreateFile(_file, 0L, 0L, WristSpeaker.GetFlag());

            //check stream
            if (Stream2 == 0)
                throw new AudioException("Stream error. Stream cannot be zero. ERROR: " + Bass.BASS_ErrorGetCode());

            //play file
            _timer = new Timer(obj => { Speaker.Play(_stream); }, null, 0, _interval);

            //wait 100 ms
//            Thread.Sleep(100);

            //play wrist stream
            Timer2 = new Timer(obj => { WristSpeaker.Play(Stream2); }, null, 100, _interval);
        }

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

        public override void Play()
        {
            base.Play();
            PlayBoth();
        }

        /// <summary>
        ///     Method that returns a string representation of the audio design.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return base.ToString() + "_BOT";
        }

        #region vars

        protected Timer Timer2;
        protected int Stream2;

        #endregion
    }
}