using System.Threading;
using AudioModule.Interfaces;
using Un4seen.Bass;

namespace AudioModule.Implementation.AudioDesigns.Constant
{
    public class ConstantWrist : ConstantIndividual
    {
        public ConstantWrist()
        {
            FeedbackType = FeedbackType.Wrist;
        }

        public ConstantWrist(string file) : base(file)
        {
            FeedbackType = FeedbackType.Wrist;
        }

        public new void PlayIndividual()
        {
            //create stream
            Stream = Bass.BASS_StreamCreateFile(File, 0L, 0L, WristSpeaker.GetFlag());

            //check stream
            if (Stream == 0)
                throw new AudioException("Stream error. Stream cannot be zero. ERROR: " + Bass.BASS_ErrorGetCode());

            //play file
            Timer = new Timer(obj => { Speaker.Play(Stream); }, null, 0, _interval);
        }
    }
}