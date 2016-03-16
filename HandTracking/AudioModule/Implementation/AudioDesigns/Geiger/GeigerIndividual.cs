using System.Threading;
using AudioModule.Interfaces;
using AudioModule.Interfaces.Designs;
using AudioModule.Interfaces.Designs.Types;

namespace AudioModule.Implementation.AudioDesigns.Geiger
{
    public class GeigerIndividual : GeigerDesign, INdividualDesign
    {
        /// <summary>
        ///     Constructor that initializes a new geiger audio design with default sounds.
        /// </summary>
        public GeigerIndividual()
        {
            FeedbackType = FeedbackType.Individual;
        }

        /// <summary>
        ///     Constructor that initializes a new geiger design with custom values.
        /// </summary>
        /// <param name="file"></param>
        public GeigerIndividual(string file) : base(file)
        {
            FeedbackType = FeedbackType.Individual;
        }

        public void PlayIndividual()
        {
            //update timer
            Timer?.Change(170, CurrentInterval);
        }

        public override void Play()
        {
            base.Play();

            //play file
            Timer = new Timer(obj => { Speaker.Play(Stream); }, null, 50, CurrentInterval);
        }

        /// <summary>
        ///     Method that sets the distance between hand and target speaker. It selects the appropriate
        ///     file to be played according to the distance and calls the Play method.
        /// </summary>
        /// <param name="distance"></param>
        public override void SetDistance(double distance)
        {
            //get interval based on distance
            var interval = GetInterval(distance);

            //if we have the same interval, we don't update the timer
            if (CurrentInterval == interval)
                return;

            CurrentInterval = interval;

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