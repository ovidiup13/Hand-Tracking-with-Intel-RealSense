using System.Windows.Controls;
using UserInterface.ViewModel;

namespace UserInterface.Views
{
    /// <summary>
    ///     Interaction logic for SpeakerSetupPage.xaml
    /// </summary>
    public partial class SpeakerSetupPage : UserControl
    {
        private readonly SpeakerSetupViewModel _speakerSetupViewModel;

        public SpeakerSetupPage()
        {
            InitializeComponent();
            _speakerSetupViewModel = new SpeakerSetupViewModel();
            this.DataContext = _speakerSetupViewModel;
        }


    }
}