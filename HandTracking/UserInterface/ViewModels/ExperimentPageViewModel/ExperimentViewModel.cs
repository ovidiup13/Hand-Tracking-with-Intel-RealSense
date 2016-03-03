using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using AudioModule.Implementation.AudioController;
using CameraModule.Interfaces.Module;
using CoreModule.Implementation;
using CoreModule.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using HandModule.Implementation.HandTracking;
using UserInterface.ViewModels.ConditionViewModels;
using UserInterface.ViewModels.HandViewModels;
using UserInterface.ViewModels.MarkerTrackingViewModels;
using UserInterface.ViewModels.SpeakerPageViewModels;

namespace UserInterface.ViewModels.ExperimentPageViewModel
{
    public class ExperimentViewModel : ViewModelBase
    {
        public ExperimentViewModel()
        {
            InitializeModules();
        }

        /// <summary>
        ///     Method which initializes all the fields and modules needed.
        /// </summary>
        private void InitializeModules()
        {
            //get modules

            //get hand tracking instance from container
            HandTracking = SimpleIoc.Default.GetInstance<HandTrackingImpl>();
            //update image source when new image is available
            HandTracking.NewImageAvailable += imageStream_NewImageAvailable;

            //get speaker controller
            SpeakerController = SimpleIoc.Default.GetInstance<SpeakerControllerImpl>();

            //get view models
            ConditionViewModel = SimpleIoc.Default.GetInstance<ConditionSetupViewModel>();
            MarkerViewModel = SimpleIoc.Default.GetInstance<MarkerTrackingViewModel>();
            ConditionViewModel = SimpleIoc.Default.GetInstance<ConditionSetupViewModel>();
            SpeakerViewModel = SimpleIoc.Default.GetInstance<SpeakerSetupViewModel>();
        }

        /// <summary>
        ///     Method which gathers the speaker list, condition and participant from the view models.
        ///     TODO: validate data and show messages before running the experiment
        /// </summary>
        private void GetData()
        {
            //get the speakers and remove the wrist
            SpeakerList = SpeakerController.Speakers.ToList();
            var wristSpeakerIndex = SpeakerList.Count - 1;
            SpeakerController.AudioSettings.WristSpeaker = SpeakerList[wristSpeakerIndex];
            SpeakerList.RemoveAt(wristSpeakerIndex); //remove the wrist

            ConditionGroups = ConditionViewModel.ConditionsGroups.ToList();
            Participant = HandViewModel.Participant;
        }

        /// <summary>
        ///     Method that updates the image on the view when a new frame is available.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void imageStream_NewImageAvailable(object sender, Tracking.NewImageArgs args)
        {
            ExperimentImageSource = ImageUtils.ConvertBitmapToWpf(args.Bitmap);
        }

        #region vars

        /// <summary>
        ///     Marker Tracking Image
        /// </summary>
        private ImageSource _experimentImageSource;
        public ImageSource ExperimentImageSource
        {
            get { return _experimentImageSource; }
            set
            {
                _experimentImageSource = value;
                RaisePropertyChanged(nameof(ExperimentImageSource));
            }
        }

        private MainExperiment MainExperiment { get; set; }
        private HandTrackingImpl HandTracking { get; set; }
        private SpeakerControllerImpl SpeakerController { get; set; }

        public List<SpeakerImpl> SpeakerList { get; private set; }
        public List<ConditionGroup> ConditionGroups { get; private set; }
        public Participant Participant { get; private set; }

        #endregion

        #region view models

        private ConditionSetupViewModel ConditionViewModel { get; set; }
        private MarkerTrackingViewModel MarkerViewModel { get; set; }
        private HandSetupViewModel HandViewModel { get; set; }
        private SpeakerSetupViewModel SpeakerViewModel { get; set; }

        #endregion
    }
}