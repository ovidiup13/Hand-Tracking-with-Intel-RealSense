using System;
using System.Collections.Generic;
using System.Threading;
using HandTracking.Interfaces.AudioController;
using Un4seen.Bass;

namespace HandTracking.Implementation.AudioController
{
    /*
    TODO: must check for number of speakers every time. If greater > 8, then throw an exception or smth
    TODO: add methods for displaying the names of all output devices, including their IDs
    TODO: add method for initializing an output device to be used in the main experiment
    TODO: release all audio resources (BASS) before exit
        */

    internal class SpeakerControllerImpl : SpeakerController
    {
        public SpeakerControllerImpl(Dictionary<int, PXCMPoint3DF32> speakerLocations)
        {
            //bass.net registration
            BassNet.Registration("ovidiu.popoviciu@hotmail.co.uk", "2X2417830312420");

            if (!Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
            {
                throw new Exception("An error occurred while initializing the BASS library.");
            }

            Bass.BASS_SetVolume(0.2f);
            Console.WriteLine(@"Volume is : " + Bass.BASS_GetVolume());

            //location of speakerLocations
            _speakerLocations = speakerLocations;

            //initialize the speakers
            InitializeSpeakers(speakerLocations.Count);
        }

        /// <summary>
        ///     Initializes the Speaker objects and maps them to flags.
        /// </summary>
        /// <param name="numberOfSpeakers"></param>
        private void InitializeSpeakers(int numberOfSpeakers)
        {
            Console.WriteLine(@"Initializing speakers...");

            _speakers = new List<SpeakerImpl>(numberOfSpeakers);
            _speakerIndexes = new int[numberOfSpeakers];

            var locations = new List<int>(_speakerLocations.Keys);

            for (var i = 0; i < numberOfSpeakers; i++)
            {
                _speakerIndexes[i] = i;

//                Console.WriteLine(SpeakerFlags[i].ToString());
                var id = locations[i];
                PXCMPoint3DF32 position;

                //get position
                _speakerLocations.TryGetValue(id, out position);

                //create a new speaker, add it to the list
                _speakers.Add(new SpeakerImpl(id, SpeakerFlags[i], position));
            }

            //randomize speakers indexes
            _speakerIndexes = ShuffleArray(_speakerIndexes);

            //set current speaker to zero
            _currentIndex = 0;
        }

        /// <summary>
        ///     Switches the speaker controller to the next speaker.
        /// </summary>
        public override void NextSpeaker()
        {
            _audioDesign.StopPlayback();
            _audioDesign.SetSpeaker(_speakers[_currentIndex++]);
            Thread.Sleep(2000);
        }

        /// <summary>
        ///     Method that sets a new Audio Design to the Speaker Controller.
        /// </summary>
        /// <param name="audioDesign"></param>
        public override void SetAudioDesign(AudioDesign audioDesign)
        {
            _audioDesign = audioDesign;
        }

        /// <summary>
        ///     Method that plays the sounds according to the type of Audio Design currently in place
        /// </summary>
        /// un
        public override void PlaySounds(double distance)
        {
            _audioDesign.SetDistance(distance);
            _audioDesign.Play(Volume);
        }

        /// <summary>
        ///     Method that returns the number of speakers registered.
        /// </summary>
        /// <returns></returns>
        public override int GetNumberOfSpeakers()
        {
            return _speakerLocations.Count;
        }

        /// <summary>
        ///     Signal the speaker controller that the current condition has ended.
        /// </summary>
        /// <param name="flag"></param>
        public override void SignalConditionEnded(bool flag)
        {
            _audioDesign.StopPlayback();
            _currentIndex = 0;
            _audioDesign = null;
            _speakerIndexes = ShuffleArray(_speakerIndexes);
        }


        /// <summary>
        ///     Signal the speaker controller that the current trial has ended.
        /// </summary>
        /// <param name="flag"></param>
        public override void SignalTrialEnded(bool flag)
        {
            _audioDesign.StopPlayback();
            _currentIndex = 0;
            _speakerIndexes = ShuffleArray(_speakerIndexes);
        }

        /// <summary>
        /// Method that returns the position of the target speaker.
        /// </summary>
        /// <returns></returns>
        public override PXCMPoint3DF32 GetSpeakerPosition()
        {
            return _audioDesign.GetSpeakerPosition();
        }

        /// <summary>
        /// Method that sets the current distance between hand and target speaker.
        /// </summary>
        /// <param name="distance"></param>
        public override void SetDistance(double distance)
        {
            _audioDesign.SetDistance(distance);
        }

        /// <summary>
        ///     Method that returns a reshuffled array of indexes.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private int[] ShuffleArray(int[] array)
        {
            var r = new Random();
            for (var i = array.Length; i > 0; i--)
            {
                var j = r.Next(i);
                var k = array[j];
                array[j] = array[i - 1];
                array[i - 1] = k;
            }
            return array;
        }

        #region vars

        //speaker locations
        private readonly Dictionary<int, PXCMPoint3DF32> _speakerLocations;

        //list of Speaker instances
        private List<SpeakerImpl> _speakers;
        private int[] _speakerIndexes;
        private int _currentIndex;

        //BASSFlags
        private static readonly List<BASSFlag> SpeakerFlags = new List<BASSFlag>
        {
            BASSFlag.BASS_SPEAKER_REARRIGHT, //8
            BASSFlag.BASS_SPEAKER_LFE, //4
            BASSFlag.BASS_SPEAKER_FRONTRIGHT, //2
            BASSFlag.BASS_SPEAKER_CENTER, //3
            BASSFlag.BASS_SPEAKER_FRONTLEFT, //1
            BASSFlag.BASS_SPEAKER_REARLEFT, //7
            BASSFlag.BASS_SPEAKER_RIGHT, // 5
            BASSFlag.BASS_SPEAKER_LEFT // 6
        };

        //audio design 
        private AudioDesign _audioDesign;

        #endregion
    }
}