using System.Collections.Generic;
using System.Threading;
using AudioModule.Interfaces;
using AudioModule.Interfaces.Designs.Types;
using Un4seen.Bass;

namespace AudioModule.Implementation.AudioDesigns.Pitch
{
    public class PitchWrist : PitchIndividual
    {
        /// <summary>
        /// </summary>
        public PitchWrist()
        {
            FeedbackType = FeedbackType.Wrist;
        }

        /// <summary>
        /// </summary>
        /// <param name="files"></param>
        public PitchWrist(List<string> files) : base(files)
        {
            FeedbackType = FeedbackType.Wrist;
        }

        /// <summary>
        /// </summary>
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
            Timer = new Timer(obj => { WristSpeaker.Play(Stream); }, null, 0, Rate);
        }

        /// <summary>
        /// </summary>
        public override void Play()
        {
            PlayIndividual();
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return base.ToString() + "_WRIST";
        }
    }
}