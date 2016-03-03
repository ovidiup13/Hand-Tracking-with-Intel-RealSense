using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using AudioModule.Interfaces;
using CameraModule.Annotations;
using CameraModule.Implementation.MarkerTracking;
using Un4seen.Bass;

namespace AudioModule.Implementation.AudioController
{
    /*
    TODO: add methods for displaying the names of all output devices, including their IDs
    TODO: release all audio resources (BASS) before exit
    */

    public class SpeakerControllerImpl : SpeakerController, INotifyPropertyChanged
    {
        /// <summary>
        ///     No args constructor initializes the default soundcard and sets a default volume.
        /// </summary>
        public SpeakerControllerImpl()
        {
            InitializeSoundCard(DefaultSoundCard);
            SetVolume(DefaultVolume);

            //initialize settings
            AudioSettings = new AudioSettingsImpl();

            //initialize speakers
            Speakers = new ObservableCollection<SpeakerImpl> {AudioSettings.WristSpeaker};
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Initializes the Speaker objects and maps them to flags. Iterates through all the speakers and checks if each marker
        ///     is assigned.
        ///     If a marker is assigned, it updates its 2D and 3D position. If a marker is not assigned, a new speaker object is
        ///     added to the speaker collection.
        ///     If there are speakers remaining not matching any of the markers from the updated list, they are removed.
        /// </summary>
        /// <param name="markers">The list of markers</param>
        public void InitializeSpeakers(List<Marker> markers)
        {
            Console.WriteLine(@"Initializing speakers...");
            var numberOfSpeakers = markers.Count;

            //clear speakers
            Speakers.Clear();

            //check if number of flags is ok
            if (numberOfSpeakers > SpeakerFlags.Count)
            {
                throw new AudioException(
                    "Too many markers detected. The software can only hold a maximum of " +
                    SpeakerFlags.Count + " numbers of markers.");
            }

            //update speaker indexes
            _speakerIndexes = new int[numberOfSpeakers];

            for (var i = 0; i < numberOfSpeakers; i++)
            {
                _speakerIndexes[i] = i;
                _speakers.Add(new SpeakerImpl(markers[i], SpeakerFlags[i]));
            }

            //re-add the wrist speaker
            Speakers.Add(AudioSettings.WristSpeaker);

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
            _targetSpeaker = _speakers[_currentIndex++];
            _audioDesign.SetSpeaker(_targetSpeaker);
            Thread.Sleep(2000);
        }

        /// <summary>
        ///     Method that plays a sound through the target speaker indicating that the current feedback has ended.
        /// </summary>
        public override void PlayConfirm()
        {
            _targetSpeaker?.PlayConfirm();
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
        public override void PlaySounds()
        {
            _audioDesign?.Play();
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
            _targetSpeaker = null;
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
            _targetSpeaker = null;
            _speakerIndexes = ShuffleArray(_speakerIndexes);
        }

        /// <summary>
        ///     Method that returns the position of the target speaker.
        /// </summary>
        /// <returns></returns>
        public override PXCMPoint3DF32 GetSpeakerPosition()
        {
            return _audioDesign.GetSpeakerPosition();
        }

        /// <summary>
        ///     Method that returns the id of the target speaker.
        /// </summary>
        /// <returns></returns>
        public override string GetSpeakerId()
        {
            return _audioDesign.GetSpeakerId();
        }

        /// <summary>
        ///     Method that sets the current distance between hand and target speaker.
        /// </summary>
        /// <param name="distance"></param>
        public override void SetDistance(double distance)
        {
            _audioDesign.SetDistance(distance);
        }

        /// <summary>
        ///     Method that returns the id of the speaker which is physically closest to the hand position
        ///     at the time of pressing space.
        /// </summary>
        /// <param name="handLocation"></param>
        /// <returns></returns>
        public override string GetClosest(PXCMPoint3DF32 handLocation)
        {
            double max = -1;
            var id = "";
            foreach (var speaker in _speakers)
            {
                var d = GetDistance(handLocation, speaker.GetPosition());
                if (max < d)
                {
                    max = d;
                    id = speaker.GetFlag().ToString();
                }
            }

            return id;
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

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region vars

        //speaker locations
        private List<Marker> _speakerLocations;

        public List<Marker> SpeakerLocations
        {
            get { return _speakerLocations; }
            set
            {
                if (value == null) return;

                _speakerLocations = value;
                InitializeSpeakers(_speakerLocations);
                OnPropertyChanged(nameof(SpeakerLocations));
            }
        }

        //list of Speaker instances
        private ObservableCollection<SpeakerImpl> _speakers;

        public ObservableCollection<SpeakerImpl> Speakers
        {
            get { return _speakers; }
            set
            {
                if (value != null && value.Count > 0)
                {
                    _speakers = value;
                    OnPropertyChanged(nameof(Speakers));
                }
            }
        }

        private int[] _speakerIndexes;
        private int _currentIndex;
        private Speaker _targetSpeaker;

        //BASSFlags
        public readonly List<BASSFlag> SpeakerFlags = new List<BASSFlag>
        {
            BASSFlag.BASS_SPEAKER_REAR2RIGHT, //8
            BASSFlag.BASS_SPEAKER_REAR2LEFT, //7
            BASSFlag.BASS_SPEAKER_REARRIGHT, //6
            BASSFlag.BASS_SPEAKER_REARLEFT, //5
            BASSFlag.BASS_SPEAKER_LFE, //4
            BASSFlag.BASS_SPEAKER_CENTER, //3
            BASSFlag.BASS_SPEAKER_FRONTLEFT, //1???
            BASSFlag.BASS_SPEAKER_FRONTRIGHT //2???
        };

        //audio design 
        private AudioDesign _audioDesign;

        #endregion
    }
}