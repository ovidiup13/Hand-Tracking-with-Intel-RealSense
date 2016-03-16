using System.Threading;
using AudioModule.Interfaces;
using AudioModule.Interfaces.Designs;
using AudioModule.Interfaces.Designs.Types;
using Un4seen.Bass;

namespace AudioModule.Implementation.AudioDesigns.Constant
{
    public class ConstantCoalescent : ConstantDesign, ICoalescentDesign
    {
        /// <summary>
        ///     Initializes a new constant design.
        /// </summary>
        public ConstantCoalescent()
        {
            FeedbackType = FeedbackType.Coalescent;
        }

        /// <summary>
        ///     Constructor with custom parameter.
        /// </summary>
        /// <param name="filePath"></param>
        public ConstantCoalescent(string filePath) : base(filePath)
        {
            FeedbackType = FeedbackType.Coalescent;
        }

        public void PlayBoth()
        {
            //create stream
            Stream2 = Bass.BASS_StreamCreateFile(File, 0L, 0L, WristSpeaker.GetFlag());

            //check stream
            if (Stream2 == 0)
                throw new AudioException("Stream error. Stream cannot be zero. ERROR: " + Bass.BASS_ErrorGetCode());

            //play file
            Timer = new Timer(obj => { Speaker.Play(Stream); }, null, 0, _interval);

            //wait 100 ms
//            Thread.Sleep(100);

            //play wrist stream
            Timer2 = new Timer(obj => { Speaker.Play(Stream2); }, null, 100, _interval);
        }

        public override void StopPlayback()
        {
            base.StopPlayback();
            Timer2?.Dispose();
            Timer2 = null;
            if (Stream2 != 0)
            {
                Speaker.StopPlayback(Stream2);
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