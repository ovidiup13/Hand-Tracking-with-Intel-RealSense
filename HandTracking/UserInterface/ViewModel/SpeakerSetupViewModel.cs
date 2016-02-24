using System.Collections.ObjectModel;
using AudioModule.Implementation.AudioController;
using GalaSoft.MvvmLight;
using MarkerTracking.Implementation;

namespace UserInterface.ViewModel
{
    public class SpeakerSetupViewModel : ViewModelBase
    {

        public SpeakerSetupViewModel()
        {
            _speakerController = new SpeakerControllerImpl();
        }


        #region data structures

        private ObservableCollection<Marker> _detectedMarkers;
        public ObservableCollection<Marker> Markers
        {
            get { return _detectedMarkers; }
            set { _detectedMarkers = value; }
        }

        #endregion

        #region modules

        private SpeakerControllerImpl _speakerController;

        #endregion

    }
}