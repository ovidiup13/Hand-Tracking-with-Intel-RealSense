using System.Threading;
using AudioModule.Interfaces;
using AudioModule.Interfaces.Designs;

namespace AudioModule.Implementation.AudioDesigns.Constant
{
    public class ConstantIndividual : ConstantDesign, INdividualDesign
    {
        /// <summary>
        ///     Initializes a new constant design.
        /// </summary>
        public ConstantIndividual()
        {
        }

        /// <summary>
        ///     Constructor with custom parameter.
        /// </summary>
        /// <param name="filePath"></param>
        public ConstantIndividual(string filePath) : base(filePath)
        {
        }


        public void PlayIndividual()
        {
            //play file
            Timer = new Timer(obj => { Speaker.Play(Stream); }, null, 200, _interval);
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