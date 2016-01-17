using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using OpenCV.Net;
using Aruco.Net;
using HandTracking.Implementation.AudioController;
using HandTracking.Implementation.AudioDesigns;
using HandTracking.Implementation.Core;
using HandTracking.Implementation.MarkerTracking;
using HandTracking.Interfaces.Core;
using HandTracking.Interfaces.Module;
using Un4seen.Bass;
using Size = OpenCV.Net.Size;

namespace HandTracking
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region vars

        private IExperiment _mainExperiment;
        private static int _numberOfTrials = 2;

        #endregion

        /// <summary>
        /// Region that holds the variables for the marker tracking module.
        /// </summary>
        #region marker tracking vars

        private MarkerTrackingModule _markerTrackingModule;
        private Tracking _markerTracking;
        private MarkerData _markerData;

        #endregion

        public MainWindow()
        {
            InitializeComponent(); 
        }

        private void StartExperiment()
        {
            //create a list of conditions
            List<ConditionImpl> conditions = new List<ConditionImpl>();
            for (int i = 0; i < 2; i++)
            {
                var condition = new ConditionImpl(_numberOfTrials);
                conditions.Add(condition);
            }

            //pass these to main experiment
            _mainExperiment = new MainExperiment(conditions.ToArray(), new SpeakerController());

            //start experiment
            _mainExperiment.StartExperiment();
        }

        /// <summary>
        /// Method that initializes the marker tracking module and starts marker tracking process.
        /// </summary>
        private void StartMarkerTracking()
        {
            _markerTrackingModule = new MarkerTrackingModule();
            _markerTracking = _markerTrackingModule.GetInstance();
            _markerTracking.InitializeCameraModules();
            _markerData = _markerTracking.GetData() as MarkerData;

            _markerTracking.StartProcessing();
        }

        /// <summary>
        /// Method that stops Marker tracking process,
        /// </summary>
        private void StopMarkerTracking()
        {
            _markerTracking.StopProcessing();
        }

        /// <summary>
        /// Method that is called when a key is pressed on the keyboard during the experiment.
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
        /// Method that stops the main experiment and shuts down the system when the user closes the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            //if the experiment is running, then stop it
            if(_mainExperiment.IsStarted())
                _mainExperiment.StopExperiment();

            //shutdown application
            Application.Current.Shutdown();
        }

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
    }
}