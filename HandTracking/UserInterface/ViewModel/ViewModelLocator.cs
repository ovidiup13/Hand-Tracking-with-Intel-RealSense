/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:UserInterface"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace UserInterface.ViewModel
{
    /// <summary>
    ///     This class contains static references to all the view models in the
    ///     application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        ///     Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<MarkerTrackingViewModel>();
            SimpleIoc.Default.Register<SpeakerSetupViewModel>();
            SimpleIoc.Default.Register<ConditionSetupViewModel>();
            SimpleIoc.Default.Register<HandSetupViewModel>();
            SimpleIoc.Default.Register<ExperimentViewModel>();
        }

        //main view model
        public MainViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

        //marker tracking view model
        public static MarkerTrackingViewModel MarkerViewModel
        {
            get { return ServiceLocator.Current.GetInstance<MarkerTrackingViewModel>(); }
        }

        //marker tracking view model
        public SpeakerSetupViewModel SpeakerViewModel
        {
            get { return ServiceLocator.Current.GetInstance<SpeakerSetupViewModel>(); }
        }

        //marker tracking view model
        public ConditionSetupViewModel ConditonViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ConditionSetupViewModel>(); }
        }

        //marker tracking view model
        public HandSetupViewModel HandViewModel
        {
            get { return ServiceLocator.Current.GetInstance<HandSetupViewModel>(); }
        }

        //marker tracking view model
        public ExperimentViewModel ExperimentViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ExperimentViewModel>(); }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}