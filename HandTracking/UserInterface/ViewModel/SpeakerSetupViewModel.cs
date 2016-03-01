using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using AudioModule.Implementation.AudioController;
using AudioModule.Interfaces;
using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Controls;
using GalaSoft.MvvmLight;
using MarkerTracking.Implementation;
using Un4seen.Bass;

namespace UserInterface.ViewModel
{
    public class SpeakerSetupViewModel : ViewModelBase
    {
        #region modules

        private readonly SpeakerControllerImpl _speakerController;

        #endregion

        public SpeakerSetupViewModel()
        {
            _speakerController = new SpeakerControllerImpl();
            SpeakerCollection = new ObservableCollection<SpeakerImpl>();
            SpeakerCollection.Add(new SpeakerImpl(new Marker(1), BASSFlag.BASS_SPEAKER_FRONT));
            SpeakerCollection.Add(new SpeakerImpl(new Marker(2), BASSFlag.BASS_SPEAKER_FRONT));
            SpeakerCollection.Add(new SpeakerImpl(new Marker(3), BASSFlag.BASS_SPEAKER_FRONT));

            //initialize flags
            InitializeSpeakerFlags();
        }

        /// <summary>
        /// Method that initializes the speaker flags to be bound to the UI.
        /// </summary>
        private void InitializeSpeakerFlags()
        {
            SpeakerFlags = new List<BASSFlag>();
            foreach (var flag in _speakerController.SpeakerFlags)
            {
                SpeakerFlags.Add(flag);
            }
        }

        /// <summary>
        ///     Method that initializes the speakers and retrieves the list.
        /// </summary>
        private void InitializeSpeakers()
        {
            try
            {
                //initialize new speakers
                _speakerController.InitializeSpeakers(_detectedMarkers.Count);
                SpeakerCollection = new ObservableCollection<SpeakerImpl>(_speakerController.Speakers);
            }
            catch (AudioException audioException)
            {
                var messageBox = MessageBoxButton.OK;
                ModernDialog.ShowMessage(audioException.Message, "Error", messageBox);
                Console.WriteLine(audioException.StackTrace);
            }
        }

        #region data structures

        private SpeakerFlag _someFlag;
        public SpeakerFlag SomeFlag
        {
            get { return _someFlag; }
            set
            {
                _someFlag = value;
                Console.WriteLine(_someFlag);
                RaisePropertyChanged("SomeFlag");
            }
        }

        private ObservableCollection<Marker> _detectedMarkers;
        public ObservableCollection<Marker> Markers
        {
            get { return _detectedMarkers; }
            set
            {
                if (value == null) return;

                _detectedMarkers = value;
                RaisePropertyChanged("Markers");
                InitializeSpeakers();
            }
        }

        private ObservableCollection<SpeakerImpl> _speakerCollection;
        public ObservableCollection<SpeakerImpl> SpeakerCollection
        {
            get { return _speakerCollection; }
            set
            {
                if (value == null) return;
                _speakerCollection = value;
                RaisePropertyChanged("SpeakerCollection");
            }
        }

        /// <summary>
        ///     Field that holds the speaker flags which are bound to the UI.
        /// </summary>
        private List<BASSFlag> _speakerFlags;
        public List<BASSFlag> SpeakerFlags
        {
            get { return _speakerFlags; }
            set
            {
                if (value == null) return;
                _speakerFlags = value;
                RaisePropertyChanged("SpeakerFlags");
            }
        }

        #endregion
    }

    public class SpeakerFlag
    {
        public BASSFlag Flag { get; set; }
    }
}