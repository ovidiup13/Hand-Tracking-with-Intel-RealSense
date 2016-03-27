using System;
using System.Collections.Generic;
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

        #region modules

        public SpeakerControllerImpl SpeakerController { get; }

        #endregion

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

        #region data structures

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

        #endregion
    }
}