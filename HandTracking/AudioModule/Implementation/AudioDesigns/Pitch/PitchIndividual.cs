using System.Collections.Generic;
using System.Threading;
using AudioModule.Interfaces;
using AudioModule.Interfaces.Designs;
using Un4seen.Bass;

namespace AudioModule.Implementation.AudioDesigns.Pitch
{
    internal class PitchIndividual : PitchDesign, INdividualDesign
    {
        /// <summary>
        /// </summary>
        /// <param name="files"></param>
        public PitchIndividual(List<string> files) : base(files)
        {
        }

        /// <summary>
        /// </summary>
        public PitchIndividual()
        {
        }

        public void PlayIndividual()
        {
            //stop the playback and change to other file
            StopPlayback();

            //create stream
            Stream = Bass.BASS_StreamCreateFile(CurrentFile, 0L, 0L, Speaker.GetFlag());

            //check stream
            if (Stream == 0)
                throw new AudioException("Stream error. Stream cannot be zero. ERROR: " + Bass.BASS_ErrorGetCode());

            //play file
            Timer = new Timer(obj => { Speaker.Play(Stream); }, null, 0, Rate);
        }

        public override void Play()
        {
           base.Play();

            //play file
            PlayIndividual();
        }

        /// <summary>
        ///     Method that sets the distance between hand and target speaker. It selects the appropriate
        ///     file to be played according to the distance and calls the Play method.
        /// </summary>
        /// <param name="distance"></param>
        public override void SetDistance(double distance)
        {
            //get file
            var file = GetFile(distance);

            //if we have the same file, then don't need to change feedback
            if (CurrentFile != null && CurrentFile == file)
            {
                //                Console.WriteLine(@"Skipped current file: " + _currentFile);
                return;
            }

            //otherwise change the file
            CurrentFile = file;

            //play next sound
            PlayIndividual();
        }

        /// <summary>
        ///     Method that returns the string representation of the audio design.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return base.ToString() + "_IND";
        }
    }
}