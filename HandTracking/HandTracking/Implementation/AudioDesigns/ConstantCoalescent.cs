using System;
using System.Threading;
using HandTracking.Interfaces.AudioController;
using HandTracking.Interfaces.AudioController.Designs;
using Un4seen.Bass;

namespace HandTracking.Implementation.AudioDesigns
{
    internal class ConstantCoalescent : ConstantDesign, ICoalescentDesign
    {

        /// <summary>
        ///     Initializes a new constant design.
        /// </summary>
        public ConstantCoalescent()
        {
            _file = "Sounds\\Pluck\\obj8p.wav";
            CheckFile(_file);
        }

        /// <summary>
        ///     Constructor with custom parameter.
        /// </summary>
        /// <param name="filePath"></param>
        public ConstantCoalescent(string filePath)
        {
            _file = filePath;
            CheckFile(_file);
        }

        public void PlayBoth()
        {
            //create stream
            _stream2 = Bass.BASS_StreamCreateFile(_file, 0L, 0L, WristSpeaker.GetFlag());

            //check stream
            if (_stream2 == 0)
                throw new AudioException("Stream error. Stream cannot be zero. ERROR: " + Bass.BASS_ErrorGetCode());

            //play file
            _timer = new Timer(obj => { Speaker.Play(_stream); }, null, 200, _interval);

            //wait 100 ms
            Thread.Sleep(100);

            //play wrist stream
            _timer2 = new Timer(obj => { WristSpeaker.Play(_stream2); }, null, 200, _interval);

        }

        public override void StopPlayback()
        {
            base.StopPlayback();
            _timer2?.Dispose();
            _timer2 = null;
            if (_stream2 != 0)
            {
                WristSpeaker?.StopPlayback(_stream2);
                _stream2 = 0;
            }

        }

        public override void Play()
        {
            base.Play();
            PlayBoth();
        }

        #region vars

        private Timer _timer2;
        private int _stream2;

        #endregion
    }
}