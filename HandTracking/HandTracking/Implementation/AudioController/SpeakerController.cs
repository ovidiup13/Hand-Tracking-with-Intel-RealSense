using System;
using System.Collections.Generic;
using HandTracking.Interfaces.AudioController;
using Un4seen.Bass;

namespace HandTracking.Implementation.AudioController
{
    /*
    TODO: must check for number of speakers every time. If greater > 8, then throw an exception or smth
        */
    internal class SpeakerController : Interfaces.AudioController.SpeakerController
    {
        public SpeakerController(Dictionary<int, PXCMPoint3DF32> speakerLocations)
        {
            //bass.net registration
            BassNet.Registration("ovidiu.popoviciu@hotmail.co.uk", "2X2417830312420");

            //location of speakerLocations
            _speakerLocations = speakerLocations;

            //initialize the speakers
            InitializeSpeakers(speakerLocations.Count);
        }

        /// <summary>
        /// Initializes the Speaker objects and maps them to flags. 
        /// 
        /// 
        /// </summary>
        /// <param name="numberOfSpeakers"></param>
        private void InitializeSpeakers(int numberOfSpeakers)
        {
            Console.WriteLine(@"Initializing speakers...");
            _speakers = new List<Speaker>(GetNumberOfSpeakers());
            _speakerIndexes = new int[numberOfSpeakers];
            for (var i = 0; i < numberOfSpeakers; i++)
            {
                _speakerIndexes[i] = i;
                Console.WriteLine(SpeakerFlags[i].ToString());
                _speakers[i] = new SpeakerImpl(SpeakerFlags[0]);
            }
            
            //randomize speakers indexes
            _speakerIndexes = ShuffleArray(_speakerIndexes);
            //set current speaker to zero
            _currentIndex = 0;
        }

        /// <summary>
        /// Switches the speaker controller to the next speaker.
        /// </summary>
        public override void NextSpeaker()
        {
            _audioDesign.SetSpeaker(_speakers[_currentIndex++]);
        }

        /// <summary>
        /// Method that sets a new Audio Design to the Speaker Controller.
        /// </summary>
        /// <param name="audioDesign"></param>
        public override void SetAudioDesign(IAudioDesign audioDesign)
        {
            _audioDesign = audioDesign;
        }

        /// <summary>
        /// Method that plays the sounds according to the type of Audio Design currently in place
        /// </summary>un
        public override void PlaySounds(double distance)
        {
            _audioDesign.Play(distance);
        }

        /// <summary>
        /// Method that returns the number of speakers registered.
        /// </summary>
        /// <returns></returns>
        public override int GetNumberOfSpeakers()
        {
            return _speakerLocations.Count;
        }

        public override void SignalConditionEnded(bool flag)
        {
            _currentIndex = 0;
            _audioDesign = null;
            _speakerIndexes = ShuffleArray(_speakerIndexes);
        }

        int[] ShuffleArray(int[] array)
        {
            Random r = new Random();
            for (int i = array.Length; i > 0; i--)
            {
                int j = r.Next(i);
                int k = array[j];
                array[j] = array[i - 1];
                array[i - 1] = k;
            }
            return array;
        }

        #region vars

        //speaker locations
        private readonly Dictionary<int, PXCMPoint3DF32> _speakerLocations;

        //list of Speaker instances
        private List<Speaker> _speakers;
        private int[] _speakerIndexes;
        private int _currentIndex;

        //BASSFlags
        private static readonly List<BASSFlag> SpeakerFlags = new List<BASSFlag>
        {
            BASSFlag.BASS_SPEAKER_REARRIGHT, //8
            BASSFlag.BASS_SPEAKER_LFE, //4
            BASSFlag.BASS_SPEAKER_FRONTRIGHT, //2
            BASSFlag.BASS_SPEAKER_CENTER,//3
            BASSFlag.BASS_SPEAKER_FRONTLEFT, //1
            BASSFlag.BASS_SPEAKER_REARLEFT, //7
            BASSFlag.BASS_SPEAKER_RIGHT, // 5
            BASSFlag.BASS_SPEAKER_LEFT // 6
        };

        //audio design 
        private IAudioDesign _audioDesign;

        #endregion

        /* public override void SetSpeakers(List<Interfaces.AudioController.SpeakerImpl> speakerLocations)
         {
             throw new NotImplementedException();
         }

         public override List<Interfaces.AudioController.SpeakerImpl> GetAllSpeakers()
         {
             throw new NotImplementedException();
         }

         public void SetVolume(float volume)
         {
             throw new NotImplementedException();
         }

         public void StopStreaming()
         {
             throw new NotImplementedException();
         }*/
    }
}