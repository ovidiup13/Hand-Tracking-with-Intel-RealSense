using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using AudioModule.Implementation.AudioController;
using AudioModule.Interfaces;
using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Controls;
using GalaSoft.MvvmLight;
using Un4seen.Bass;

namespace UserInterface.ViewModels
{
    public class SpeakerSetupViewModel : ViewModelBase
    {
        //TODO: add the test command for each speaker to the grid
        //TODO: tie the speaker initialization method to the marker tracking panel]

        public SpeakerSetupViewModel(SpeakerControllerImpl speakerController,
            MarkerTrackingViewModel markerTrackingViewModel)
        {
            SpeakerController = speakerController;

            //set initialize speakers to run every time new markers are available
            markerTrackingViewModel.NewMarkersAvailableEvent += InitializeSpeakers;

            //initialize flags
            InitializeSpeakerFlags();

            TestSoundCommand = new RelayCommand(TestSound);
            InitSoundCardCommand = new RelayCommand(InitSoundCard, CanInitDevice);
        }

        #region modules

        public SpeakerControllerImpl SpeakerController { get; }

        #endregion

        /// <summary>
        ///     Checks whether the sound card device can be initialized.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool CanInitDevice(object arg)
        {
            return SelectedSoundDevice != null;
        }

        /// <summary>
        ///     Initialize the current sound card
        /// </summary>
        /// <param name="obj"></param>
        private void InitSoundCard(object obj)
        {
            if (SelectedSoundDevice == null) return;
            try
            {
                SpeakerController.AudioSettings.InitializeSoundDevice(SelectedSoundDevice.Id, Frequency);
                var messageBox = MessageBoxButton.OK;
                ModernDialog.ShowMessage("Sound Device " + SelectedSoundDevice.Name + " has been successfully initialized. Please make sure to test the speakers before continuing.", "Init Successful", messageBox);
                InitMessage = "Initialization Successful!";
            }
            catch (AudioException audioException)
            {
                var messageBox = MessageBoxButton.OK;
                ModernDialog.ShowMessage(audioException.Message, "Error", messageBox);
                Console.WriteLine(audioException.StackTrace);
                InitMessage = "Initialization Failed!";
            }
        }

        private static void TestSound(object o)
        {
            //cast parameter to BASSFlag, otherwise to BASS DEFAULT flag
//            BASSFlag flag = o as BASSFlag? ?? BASSFlag.BASS_DEFAULT;
            var speaker = o as SpeakerImpl;

            try
            {
                speaker?.Test();
            }
            catch (AudioException audioException)
            {
                var messageBox = MessageBoxButton.OK;
                ModernDialog.ShowMessage(audioException.Message, "Error", messageBox);
                Console.WriteLine(audioException.StackTrace);
            }
        }

        /// <summary>
        ///     Method that initializes the speaker flags to be bound to the UI.
        /// </summary>
        private void InitializeSpeakerFlags()
        {
            SpeakerFlags = new List<BASSFlag>();
            foreach (var flag in SpeakerController.AudioSettings.SpeakerFlags)
            {
                SpeakerFlags.Add(flag);
            }
        }

        /// <summary>
        ///     Method that initializes the speakers and retrieves the list.
        /// </summary>
        private void InitializeSpeakers(object sender, MarkerTrackingViewModel.NewMarkersArgs markerArgs)
        {
            try
            {
                if (SpeakerController.Speakers == null)
                {
                    SpeakerController.InitializeSpeakers(markerArgs.Markers);
                }
                else if (SpeakerController.Speakers.Count > 0)
                {
                    SpeakerController.UpdateSpeakers(markerArgs.Markers);
                }
            }
            catch (AudioException audioException)
            {
                var messageBox = MessageBoxButton.OK;
                ModernDialog.ShowMessage(audioException.Message, "Error", messageBox);
                Console.WriteLine(audioException.StackTrace);
            }
        }

        #region UI fields

        private string _initMessage = "Default SoundCard Initialized";
        public string InitMessage
        {
            get { return _initMessage;}
            set
            {
                _initMessage = value;
                RaisePropertyChanged(nameof(InitMessage));
            }
        }

        #endregion

        #region data structures

        //device held
        public Device SelectedSoundDevice { get; set; }
        public int Frequency { get; set; } = 44100;

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

        public ICommand TestSoundCommand { get; protected set; }
        public ICommand InitSoundCardCommand { get; protected set; }

        #endregion
    }
}