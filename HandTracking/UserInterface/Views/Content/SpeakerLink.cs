using FirstFloor.ModernUI.Presentation;
using UserInterface.ViewModel;

namespace UserInterface.Views.Content
{
    internal class SpeakerLink : Link
    {
        /// <summary>
        /// </summary>
        private SpeakerViewModel _speakerVm;

        public SpeakerViewModel SpeakerVm
        {
            get { return _speakerVm; }
            set
            {
                _speakerVm = value;
                OnPropertyChanged("SpeakerViewModel");
            }
        }
    }
}