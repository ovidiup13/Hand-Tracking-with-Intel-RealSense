using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioModule.Implementation.AudioController;
using GalaSoft.MvvmLight;

namespace UserInterface.ViewModel
{
    public class SpeakerViewModel : ViewModelBase
    {
        public SpeakerViewModel()
        {

        }

        private SpeakerImpl _speaker ;
        public SpeakerImpl Speaker
        {
            get
            {
                return _speaker;
            }

            set
            {
                if (value == null) return;
                _speaker = value;
                RaisePropertyChanged("Speaker");
            }
        }
    }
}
