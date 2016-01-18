using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using HandTracking.Implementation.AudioController;
using HandTracking.Implementation.Core;
using HandTracking.Implementation.MarkerTracking;
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
            InitializeComponent();
        }

        /// <summary>
        ///     Method that starts the experiment.
        /// </summary>
        /// <param name="markers">Dictionary of markers passed to SpeakerController (i.e. location of speakers)</param>
        private void StartExperiment(Dictionary<int, PXCMPoint3DF32> markers)
        {
            //TODO: conditions must be initialized in another method
            //create a list of conditions
            var conditions = new List<ConditionImpl>();
            for (var i = 0; i < 2; i++)
            {
                var condition = new ConditionImpl(_numberOfTrials);
                conditions.Add(condition);
            }

            //pass these to main experiment
            //TODO: check marker data for null
            _mainExperiment = new MainExperiment(conditions.ToArray(), new SpeakerController(markers));

            //start experiment
            _mainExperiment.StartExperiment();
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

            //shutdown application
            Application.Current.Shutdown();
        }

        #region experiment vars

        private IExperiment _mainExperiment;
        private static readonly int _numberOfTrials = 2;

        #endregion

        #region window vars

        #endregion

        /* void imageStream_NewImageAvailable(object sender, RealSenseImageStream.NewImageArgs args)
       {
           Dispatcher.Invoke(new Action(() => { imageComponent.Source = ImageUtils.ConvertBitmapToWpf(args.Bitmap); }));
       }

       private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
       {
           // Avoiding problems with Dispatcher
           imageStream.NewImageAvailable -= imageStream_NewImageAvailable;
           imageStream.StopStream();
           handsLocation.StopTracking();
       }*/

        private void Quit_Experiment(object sender, RoutedEventArgs e)
        {
            //if the experiment is running, then stop it
            if (_mainExperiment.IsStarted())
                _mainExperiment.StopExperiment();
        }
    }
}