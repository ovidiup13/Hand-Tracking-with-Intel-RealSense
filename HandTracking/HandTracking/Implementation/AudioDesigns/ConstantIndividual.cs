using System;
using System.IO;
using System.Threading;
using HandTracking.Interfaces.AudioController;
using HandTracking.Interfaces.AudioController.Designs;
using Un4seen.Bass;

namespace HandTracking.Implementation.AudioDesigns
{
    public class ConstantIndividual : ConstantDesign, INdividualDesign
    {
        /// <summary>
        ///     Initializes a new constant design.
        /// </summary>
        public ConstantIndividual()
        {
            _file = "Sounds\\Pluck\\obj8p.wav";
            CheckFile(_file);
        }

        /// <summary>
        ///     Constructor with custom parameter.
        /// </summary>
        /// <param name="filePath"></param>
        public ConstantIndividual(string filePath)
        {
            _file = filePath;
            CheckFile(_file);
        }


        public void PlayIndividual()
        {
            //play file
            _timer = new Timer(obj => { Speaker.Play(_stream); }, null, 200, _interval);
        }

        /// <summary>
        ///     Plays the file through the speaker.
        /// </summary>
        public override void Play()
        {
            base.Play();
            PlayIndividual();
        }

        /// <summary>
        ///     Method that returns a string representation of the audio design.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return base.ToString() + "_IDV";
        }
    }
}