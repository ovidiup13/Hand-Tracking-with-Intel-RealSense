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
    */

    public class SpeakerControllerImpl : SpeakerController, INotifyPropertyChanged
    {
        /// <summary>
        ///     No args constructor initializes the default soundcard and sets a default volume.
        /// </summary>
        public SpeakerControllerImpl()
        {
            //initialize settings
            AudioSettings = new AudioSettingsImpl();

            Speakers = new ObservableCollection<Speaker>();

            IsPlaying = false;
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

            if (Speakers == null)
            {
                //initialize speaker collection
                Speakers = new ObservableCollection<Speaker> {null};
                Speakers.Clear();
            }
            else
            {
                //clear speakers
                Speakers.Clear();
            }

            //check if number of flags is ok
            if (numberOfSpeakers > AudioSettings.SpeakerFlags.Count)
            {
                throw new AudioException(
                    "Too many markers detected. The software can only hold a maximum of " +
                    AudioSettings.SpeakerFlags.Count + " numbers of markers.");
            }

            //update speaker indexes
            _speakerIndexes = new int[numberOfSpeakers];

            for (var i = 0; i < numberOfSpeakers; i++)
            {
                _speakerIndexes[i] = i;
                _speakers.Add(new SpeakerImpl(markers[i], AudioSettings.SpeakerFlags[i]));
            }

            //randomize speakers indexes
            _speakerIndexes = ShuffleArray(_speakerIndexes);

            //set current speaker to zero
            _currentIndex = 0;
            IsPlaying = false;
        }

        /// <summary>
        ///     Switches the speaker controller to the next speaker.
        /// </summary>
        public override void NextSpeaker()
        {
            _audioDesign.StopPlayback();
            _targetSpeaker = _speakers[_currentIndex++];
            _audioDesign.SetSpeaker(_targetSpeaker);
            _targetSpeaker.PlayInitialSound();

            //wait one second before starting
            Thread.Sleep(1000);
            IsPlaying = true;
        }

        /// <summary>
        ///     Method that plays a sound through the target speaker indicating that the current feedback has ended.
        /// </summary>
        public override void PlayConfirm()
        {
            if(IsPlaying)
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
            return Speakers.Count;
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
        ///     Method that stops the current playback of the audio design.
        /// </summary>
        public void StopSounds()
        {
            _audioDesign?.StopPlayback();
            IsPlaying = false;
        }

        /// <summary>
        ///     Method that tests the current audio configuration
        /// </summary>
        public static void TestSoundCard()
        {
            //initialize current sound card
//            AudioSettings.InitializeSoundCard(-1);

            //create dummy stream
            var stream = Bass.BASS_StreamCreateFile("Sounds\\Pluck\\obj8p.wav", 0L, 0L,
                BASSFlag.BASS_SPEAKER_REAR | BASSFlag.BASS_STREAM_AUTOFREE);
            if (stream == 0)
            {
                throw new AudioException("An error occurred while trying to test the soundcard." +
                                         Bass.BASS_ErrorGetCode());
            }
        }

        /// <summary>
        ///     Method that cleans up the speaker controller by stopping the current playback and resetting speaker
        ///     order.
        /// </summary>
        public override void StopPlayback()
        {
            //stop playback, if currently running
            StopSounds();
            IsPlaying = false;
            _currentIndex = 0;
            _targetSpeaker = null;
            _speakerIndexes = ShuffleArray(_speakerIndexes);
        }

        /// <summary>
        /// Method that disposes of all BASS resources.
        /// </summary>
        public override void CleanUp()
        {
            Bass.BASS_Free();
        }

        /// <summary>
        ///     Method that returns a reshuffled array of indexes.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private static int[] ShuffleArray(int[] array)
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

        //list of Speaker instances
        private ObservableCollection<Speaker> _speakers;
        public ObservableCollection<Speaker> Speakers
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

        //audio design 
        private AudioDesign _audioDesign;

        public bool IsPlaying { get; private set; }

        #endregion

        /// <summary>
        /// Method that updates the current existing markers in the speaker list.
        /// </summary>
        /// <param name="markers"></param>
        public void UpdateSpeakers(List<Marker> markers)
        {
            var ids = new List<int>();

            //update speakers from list
            for (var i = 0; i < Speakers.Count; i++)
            {
                var ok = false;
                foreach (var marker in markers)
                {
                    //update marker if already exists
                    if (Speakers[i].Marker.Id != marker.Id) continue;

                    Speakers[i].Marker.Position3D = marker.Position3D;
                    ok = true;
                    break;
                }

                if (!ok)
                {
                    ids.Add(i);
                }
            }

            //remove speakers not found in new marker list
            for (var i = ids.Count - 1; i >= 0; i--)
            {
                Speakers.RemoveAt(ids[i]);
            }

            _speakerIndexes = new int[markers.Count];

            //add new speakers
            for (var i = 0; i < markers.Count; i++)
            {
                _speakerIndexes[i] = i;

                var ok = false;
                foreach (var speaker in Speakers)
                {
                    if (markers[i].Id == speaker.Marker.Id)
                    {
                        ok = true;
                        break;
                    }
                }

                if (!ok)
                {
                    Speakers.Add(new SpeakerImpl(markers[i], AudioSettings.SpeakerFlags[i]));
                }
            }

            _speakerIndexes = ShuffleArray(_speakerIndexes);

        }
    }
}