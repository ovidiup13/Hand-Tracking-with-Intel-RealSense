using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
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
using Condition = CoreModule.Implementation.Condition;

namespace UserInterface.ViewModels
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
            PauseExperimentCommand = new RelayCommand(PauseExperiment, CanPause);

            //space pressed command
            SpacePressedCommand = new RelayCommand(o => NextTrial());

            Dispatcher.CurrentDispatcher.ShutdownStarted += ShutDownExperiment;
        }

        /// <summary>
        /// Method that stops the experiment and releases all BASS resources before shutdown.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShutDownExperiment(object sender, EventArgs e)
        {
            MainExperiment.StopExperiment();
            SpeakerController.CleanUp();
        }

        /// <summary>
        ///     Method that checks whether the experiment can be paused or not.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>Returns true if the experiment is running, false otherwise.</returns>
        private bool CanPause(object arg)
        {
            return MainExperiment.ExperimentStatus == ExperimentStatus.Running;
        }

        /// <summary>
        ///     Method that pauses the experiment.
        /// </summary>
        /// <param name="obj"></param>
        private void PauseExperiment(object obj)
        {
            MainExperiment.PauseExperiment();
        }

        /// <summary>
        ///     Method called when the user pressed the Space key on the UI. This will signal the experiment
        ///     thread to move to the next trial/speaker.
        /// </summary>
        private void NextTrial()
        {
            if (MainExperiment.ExperimentStatus != ExperimentStatus.Running) return;
            Console.WriteLine("Space pressed.");
            MainExperiment.NextTrial();
        }

        /// <summary>
        ///     Method that checks whether the experiment can be stopped or not.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>If the experiment is running or paused, the method returns true. Otherwise it returns false.</returns>
        private bool CanStop(object arg)
        {
            return MainExperiment.ExperimentStatus == ExperimentStatus.Running ||
                   MainExperiment.ExperimentStatus == ExperimentStatus.Paused;
        }

        /// <summary>
        ///     Method that stops the current experiment.
        /// </summary>
        /// <param name="obj"></param>
        private void StopExperiment(object obj)
        {
            //stop experiment
            MainExperiment.StopExperiment();
        }

        /// <summary>
        ///     Method that checks whether the experiment can be started.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>Returns true if experiment is initialized or paused. Otherwise it returns false.</returns>
        private bool CanStart(object arg)
        {
            //check if experiment is running
            return MainExperiment.ExperimentStatus != ExperimentStatus.Running;
        }

        /// <summary>
        ///     Method that starts the experiment. If the experiment is stopped or not started, it will start a new experiment.
        ///     If the experiment is paused, the method will resume the current experiment.
        /// </summary>
        /// <param name="obj"></param>
        private void StartExperiment(object obj)
        {
            //if not paused, then set up a new experiment
            if (MainExperiment.ExperimentStatus != ExperimentStatus.Paused)
            {
                var ok = GetData();
                if (!ok) return;
                MainExperiment.SetExperimentData(ConditionGroups, SpeakerController, HandTracking);
                MainExperiment.SetParticipant(Participant);

                //start experiment
                MainExperiment.StartExperiment();
            }
            else
            {
                MainExperiment.ResumeExperiment();
            }
        }

        /// <summary>
        ///     Method which gathers the speaker list, condition and participant from the view models. 
        /// </summary>
        private bool GetData()
        {
            //get the speakers and remove the wrist
            //            SpeakerList = SpeakerController.Speakers.ToList();

            if (SpeakerController.Speakers == null) //only containing wrist
            {
                var messageBox = MessageBoxButton.OK;
                ModernDialog.ShowMessage(
                    "Experiment cannot start because an error occurred when initializing the Speaker Controller.",
                    "Error", messageBox);
                return false;
            }

            //validate speaker list
            if (SpeakerController.Speakers.Count == 0) //only containing wrist
            {
                var messageBox = MessageBoxButton.OK;
                ModernDialog.ShowMessage(
                    "Experiment cannot start because speaker list in empty. Please make sure that markers have been placed in front of the camera. ",
                    "Error", messageBox);
                return false;
            }

            //test soundcard
            try
            {
                SpeakerControllerImpl.TestSoundCard();
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

        public MainExperiment MainExperiment { get; }
        private HandTrackingImpl HandTracking { get; }
        private SpeakerControllerImpl SpeakerController { get; }

        public List<SpeakerImpl> SpeakerList { get; private set; }
        public List<Condition> ConditionGroups { get; private set; }
        public Participant Participant { get; private set; }

        #endregion

        #region commands

        public ICommand StartExperimentCommand { get; set; }
        public ICommand StopExperimentCommand { get; set; }
        public ICommand PauseExperimentCommand { get; set; }
        public ICommand SpacePressedCommand { get; set; }

        #endregion

        #region view models

        private ConditionSetupViewModel ConditionViewModel { get; }
        private HandSetupViewModel HandViewModel { get; }

        #endregion
    }
}