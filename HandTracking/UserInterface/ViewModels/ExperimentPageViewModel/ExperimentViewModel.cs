using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using AudioModule.Implementation.AudioController;
using AudioModule.Interfaces;
using CameraModule.Implementation.HandTracking;
using CameraModule.Interfaces.Module;
using CameraModule.Interfaces.UI;
using CoreModule.Implementation;
using CoreModule.Interfaces;
using FirstFloor.ModernUI.Windows.Controls;
using GalaSoft.MvvmLight;
using UserInterface.ViewModels.ConditionViewModels;
using UserInterface.ViewModels.HandViewModels;

namespace UserInterface.ViewModels.ExperimentPageViewModel
{
    public class ExperimentViewModel : ViewModelBase
    {
        public ExperimentViewModel(HandTrackingImpl handTracking, SpeakerControllerImpl speakerController,
            ConditionSetupViewModel conditionSetupViewModel, HandSetupViewModel handViewModel)
        {
            //update image source when new image is available
            HandTracking = handTracking;
            HandTracking.NewImageAvailable += imageStream_NewImageAvailable;

            //get speaker controller
            SpeakerController = speakerController;

            //get view models
            ConditionViewModel = conditionSetupViewModel;

            HandViewModel = handViewModel;

            //init experiment
            MainExperiment = new MainExperiment();

            //commands
            StartExperimentCommand = new RelayCommand(StartExperiment, CanStart);
            StopExperimentCommand = new RelayCommand(StopExperiment, CanStop);

            //space pressed command
            SpacePressedCommand = new RelayCommand(o => NextTrial());
        }

        private void NextTrial()
        {
              if (MainExperiment.IsStopped())
                Console.WriteLine("Space pressed");

              MainExperiment.NextTrial();
        }

        private bool CanStop(object arg)
        {
            return MainExperiment.IsStarted();
        }

        private void StopExperiment(object obj)
        {
            //stop experiment
            MainExperiment.StopExperiment();
        }

        private bool CanStart(object arg)
        {
            //check if experiment is running
            return MainExperiment.IsStopped();
        }

        private void StartExperiment(object obj)
        {
            //get data
            if (!GetData())
                return;

            //pass data to experiment object
            MainExperiment.SetExperimentData(ConditionGroups, SpeakerController, HandTracking);

            //start experiment
            MainExperiment.StartExperiment();
        }

        /// <summary>
        ///     Method which gathers the speaker list, condition and participant from the view models.
        ///     TODO: validate data and show messages before running the experiment
        /// </summary>
        private bool GetData()
        {
            //get the speakers and remove the wrist
            SpeakerList = SpeakerController.Speakers.ToList();

            //validate speaker list
            if (SpeakerList.Count == 1) //only containing wrist
            {
                var messageBox = MessageBoxButton.OK;
                ModernDialog.ShowMessage(
                    "Experiment cannot start because speaker list in empty. Please make sure that markers have been placed in front of the camera and were detected. ",
                    "Error", messageBox);
                return false;
            }

            var wristSpeakerIndex = SpeakerList.Count - 1;
            SpeakerController.AudioSettings.WristSpeaker = SpeakerList[wristSpeakerIndex];
            SpeakerList.RemoveAt(wristSpeakerIndex); //remove the wrist

            //test soundcard
            try
            {
                SpeakerController.TestSoundCard();
            }
            catch (AudioException e)
            {
                var messageBox = MessageBoxButton.OK;
                ModernDialog.ShowMessage(
                    "Experiment cannot start because: " + e.Message, "Error", messageBox);
                return false;
            }

            ConditionGroups = ConditionViewModel.ConditionsGroups.ToList();
            if (ConditionGroups == null || ConditionGroups.Count == 0)
            {
                var messageBox = MessageBoxButton.OK;
                ModernDialog.ShowMessage(
                    "Experiment cannot start because condition list is either null or empty. Please make sure that conditions have been defined in the Conditions page. ",
                    "Info", messageBox);
                return false;
            }

            //initialize audio designs for condition
            InitializeAudioDesigns();

            //get participant
            Participant = HandViewModel.Participant;

            return Participant != null;
        }

        /// <summary>
        ///     Method that initializes the Audio Design for each condition.
        /// </summary>
        private void InitializeAudioDesigns()
        {
            foreach (var group in ConditionGroups)
            {
                foreach (var cond in group.Conditions)
                {
                    cond.AudioDesign = AudioDesignFactory.GetAudioDesign(cond.DesignType, cond.FeedbackType);
                }
            }
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

        public BitmapSource Convert(System.Drawing.Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height, 96, 96, PixelFormats.Bgr24, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);
            return bitmapSource;
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

        private MainExperiment MainExperiment { get; }
        private HandTrackingImpl HandTracking { get; }
        private SpeakerControllerImpl SpeakerController { get; }

        public List<SpeakerImpl> SpeakerList { get; private set; }
        public List<ConditionGroup> ConditionGroups { get; private set; }
        public Participant Participant { get; private set; }

        #endregion

        #region commands

        public ICommand StartExperimentCommand { get; set; }
        public ICommand StopExperimentCommand { get; set; }
        public ICommand SpacePressedCommand { get; set; }

        #endregion

        #region view models

        private ConditionSetupViewModel ConditionViewModel { get; }
        private HandSetupViewModel HandViewModel { get; }

        #endregion
    }
}