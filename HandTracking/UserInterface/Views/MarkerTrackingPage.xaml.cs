using System.Windows.Controls;
using FirstFloor.ModernUI.Windows;
using FirstFloor.ModernUI.Windows.Navigation;
using GalaSoft.MvvmLight.Ioc;
using MarkerTrackingViewModel = UserInterface.ViewModels.MarkerTrackingViewModels.MarkerTrackingViewModel;

namespace UserInterface.Views
{
    /// <summary>
    ///     Interaction logic for MarkerTrackingPage.xaml
    /// </summary>
    public partial class MarkerTrackingPage : UserControl, IContent
    {
        public MarkerTrackingPage()
        {
            InitializeComponent();
            _viewModel = SimpleIoc.Default.GetInstance<MarkerTrackingViewModel>();
            DataContext = _viewModel;
        }

        public void OnFragmentNavigation(FragmentNavigationEventArgs e)
        {
        }

        public void OnNavigatedFrom(NavigationEventArgs e)
        {
        }

        public void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        public void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
        }

        private readonly MarkerTrackingViewModel _viewModel;
    }
}