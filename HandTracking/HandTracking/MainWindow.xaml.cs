using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using HandTracking.Implementation.AudioController;
using HandTracking.Implementation.AudioDesigns;
using HandTracking.Implementation.Core;
using HandTracking.Interfaces.AudioController;
using HandTracking.Interfaces.Core;

namespace HandTracking
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow(Window mainMenu, Dictionary<int, PXCMPoint3DF32> markerLocation)
        {
            //init component
            InitializeComponent();

            //init experiment
            InitializeExperiment(markerLocation);
            StartExperiment();
        }

        /// <summary>
        /// Method that initializes a new Experiment.
        /// </summary>
        private void InitializeExperiment(Dictionary<int, PXCMPoint3DF32> markerLocation)
        {
            //TODO: conditions must be initialized in another method
            //create a list of conditions
            AudioDesign audioDesign = new ConstantAudioDesign();

            var conditions = new List<ConditionImpl>();
            for (var i = 0; i < 2; i++)
            {
                var condition = new ConditionImpl(_numberOfTrials) {AudioDesign = audioDesign};
                conditions.Add(condition);
            }

            //pass these to main experiment
            //TODO: check marker data for null
            _mainExperiment = new MainExperiment(conditions.ToArray(), new SpeakerControllerImpl(markerLocation));
        }

        /// <summary>
        /// Method that starts the experiment.
        /// </summary>
        private void StartExperiment()
        {
            //start experiment
            if (_mainExperiment != null && !_mainExperiment.IsStarted())
            {
                _mainExperiment.StartExperiment();
            }

            else
            {
                MessageBox.Show(
                    "Experiment is already started or has not been initialized.",
                    "Experiment started", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        /// <summary>
        ///     Method that is called when a key is pressed on the keyboard during the experiment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            //if we pressed space, move to next trial
            if (e.Key == Key.Space && _mainExperiment.IsStarted())
                _mainExperiment.NextTrial();
        }

        /// <summary>
        ///     Method that stops the main experiment and shuts down the system when the user closes the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            //if the experiment is running, then stop it
            if (_mainExperiment.IsStarted())
                _mainExperiment.StopExperiment();

            Application.Current.Shutdown();
        }

        #region experiment vars

        private IExperiment _mainExperiment;
        private static readonly int _numberOfTrials = 1;

        #endregion

        private void Quit_Experiment(object sender, RoutedEventArgs e)
        {
            //if the experiment is running, then stop it
            if (_mainExperiment.IsStarted())
                _mainExperiment.StopExperiment();
        }

        private void Stop_button_OnClick(object sender, RoutedEventArgs e)
        {
            //if the experiment is running, then stop it
            if (_mainExperiment.IsStarted())
                _mainExperiment.StopExperiment();
        }

        private void Start_button_OnClick(object sender, RoutedEventArgs e)
        {
            StartExperiment();
        }
    }
}