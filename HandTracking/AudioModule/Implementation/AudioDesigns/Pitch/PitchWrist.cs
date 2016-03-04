using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AudioModule.Interfaces;
using Un4seen.Bass;

namespace AudioModule.Implementation.AudioDesigns.Pitch
{
    class PitchWrist : PitchIndividual
    {

        public PitchWrist()
        {
            FeedbackType = FeedbackType.Wrist;
        }

        public PitchWrist(List<string> files) : base(files)
        {
            FeedbackType = FeedbackType.Wrist;
        }

        public override void PlayIndividual()
        {
            //stop the playback and change to other file
            StopPlayback();

            //create stream
            Stream = Bass.BASS_StreamCreateFile(CurrentFile, 0L, 0L, WristSpeaker.GetFlag());

            //check stream
            if (Stream == 0)
                throw new AudioException("Stream error. Stream cannot be zero. ERROR: " + Bass.BASS_ErrorGetCode());

            //play file
            Timer = new Timer(obj => { Speaker.Play(Stream); }, null, 0, Rate);
        }

    }
}
