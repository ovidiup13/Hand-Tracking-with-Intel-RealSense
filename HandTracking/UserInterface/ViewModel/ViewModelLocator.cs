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

            //register pages
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<MarkerTrackingViewModel>();
            SimpleIoc.Default.Register<SpeakerSetupViewModel>();
            SimpleIoc.Default.Register<ConditionSetupViewModel>();
            SimpleIoc.Default.Register<HandSetupViewModel>();
            SimpleIoc.Default.Register<ExperimentViewModel>();

            SimpleIoc.Default.Register<SpeakerViewModel>();
        }

        //main view model
        public static MainViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

        //marker tracking view model
        public static MarkerTrackingViewModel MarkerViewModel
        {
            get { return ServiceLocator.Current.GetInstance<MarkerTrackingViewModel>(); }
        }
        
        public static SpeakerSetupViewModel SpeakerViewModel
        {
            get { return ServiceLocator.Current.GetInstance<SpeakerSetupViewModel>(); }
        }

        public static ConditionSetupViewModel ConditonViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ConditionSetupViewModel>(); }
        }

        public static HandSetupViewModel HandViewModel
        {
            get { return ServiceLocator.Current.GetInstance<HandSetupViewModel>(); }
        }

        public static ExperimentViewModel ExperimentViewModel
        {
            get { return ServiceLocator.Current.GetInstance<ExperimentViewModel>(); }
        }

        public static SpeakerViewModel SpeakerModel
        {
            get { return ServiceLocator.Current.GetInstance<SpeakerViewModel>(); }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}