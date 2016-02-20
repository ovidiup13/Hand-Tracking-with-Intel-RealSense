using System;
using ClassLibrary1.Interfaces.AudioController;
using Un4seen.Bass;

namespace HandTracking.Implementation.AudioController
{
    internal class SpeakerImpl : Speaker
    {
        /// <summary>
        /// Constructor for speaker implementation.
        /// </summary>
        /// <param name="flag"></param>
        public SpeakerImpl(BASSFlag flag)
        {
            SpeakerFlag = flag;
        }

        /// <summary>
        /// Constructor for speaker implementation.
        /// </summary>
        /// <param name="flag">speaker flag</param>
        /// <param name="position">3D position</param>
        public SpeakerImpl(BASSFlag flag, PXCMPoint3DF32 position)
        {
            Position = position;
            SpeakerFlag = flag;
        }

        /// <summary>
        /// Constructor for speaker implementation.
        /// </summary>
        /// <param name="id">marker id</param>
        /// <param name="flag">speaker flag</param>
        /// <param name="position">3D position</param>
        public SpeakerImpl(int id, BASSFlag flag, PXCMPoint3DF32 position)
        {
            Id = id;
            Position = position;
            SpeakerFlag = flag;

            Console.WriteLine(@"Speaker initialized with id: " + Id + @", speaker: " + SpeakerFlag);
        }
    }
}