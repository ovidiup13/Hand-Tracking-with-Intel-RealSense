using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AudioModule.Interfaces;
using AudioModule.Interfaces.Designs;
using AudioModule.Interfaces.Designs.Types;
using Un4seen.Bass;

namespace AudioModule.Implementation.AudioDesigns.Geiger
{
    public class GeigerWrist : GeigerIndividual
    {

        public GeigerWrist()
        {
            FeedbackType = FeedbackType.Wrist;
        }

        public GeigerWrist(string file) : base(file)
        {
            FeedbackType = FeedbackType.Wrist;
        }


        public override void Play()
        {
            //check speaker
            if (Speaker == null)
                throw new NullReferenceException("Speaker cannot be null.");

            //create stream
            Stream = Bass.BASS_StreamCreateFile(File, 0L, 0L, WristSpeaker.GetFlag());

            //check stream
            if (Stream == 0)
                throw new AudioException("Stream error. Stream cannot be zero. ERROR: " + Bass.BASS_ErrorGetCode());

            Timer = new Timer(obj => { Speaker.Play(Stream); }, null, 0, CurrentInterval);
        }

        public override string ToString()
        {
            return base.ToString() + "_WRIST";
        }
    }
}
