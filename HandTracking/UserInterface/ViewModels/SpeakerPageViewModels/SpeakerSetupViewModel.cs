using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using AudioModule.Implementation.AudioController;
using AudioModule.Interfaces;
using FirstFloor.ModernUI.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Un4seen.Bass;
using UserInterface.ViewModels.MarkerTrackingViewModels;
using UserInterface.Views;

namespace UserInterface.ViewModels.SpeakerPageViewModels
{
    public class SpeakerSetupViewModel : ViewModelBase
    {
        #region modules

        public SpeakerControllerImpl SpeakerController { get; }

        #endregion

        //TODO: add the  command for each speaker to the grid
        //TODO: tie the speaker initialization method to the marker tracking panel]

        public SpeakerSetupViewModel(SpeakerControllerImpl speakerController, MarkerTrackingViewModel markerTrackingViewModel)
        {
            SpeakerController = speakerController;
            
            //set initialize speakers to run every time new markers are available
            markerTrackingViewModel.NewMarkersAvailableEvent += InitializeSpeakers;

            //initialize flags
            InitializeSpeakerFlags();
        }

        /// <summary>
        ///     Method that initializes the speaker flags to be bound to the UI.
        /// </summary>
        private void InitializeSpeakerFlags()
        {
            SpeakerFlags = new List<BASSFlag>();
            foreach (var flag in SpeakerController.SpeakerFlags)
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
                //initialize new speakers
                SpeakerController.InitializeSpeakers(markerArgs.Markers);
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

        #endregion
    }

    public class SpeakerFlag
    {
        public BASSFlag Flag { get; set; }
    }
}