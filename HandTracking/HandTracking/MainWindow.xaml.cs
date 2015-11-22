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
using HandTracking.Implementation.Core;
using HandTracking.Interfaces.Core;
using Size = OpenCV.Net.Size;

namespace HandTracking
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private RealSenseImageStream imageStream;
        private RealSenseHands handsLocation;
        private readonly IExperiment mainExperiment;

        public MainWindow()
        {
            InitializeComponent();

            //create a new trial list
            List<Trial> trialList = new List<Trial>();
            for (int i = 0; i < 5; i++)
                trialList.Add(new Trial(i));

            //create a list of conditions
            List<ConditionImpl> conditions = new List<ConditionImpl>();
            for (int i = 0; i < 2; i++)
            {
                conditions.Add(new ConditionImpl(trialList.ToArray()));
            }

            //pass these to main experiment
            mainExperiment = new MainExperiment(conditions.ToArray());

            //start experiment
            mainExperiment.StartExperiment();
        }

        /// <summary>
        /// Method that is called when a key is pressed on the keyboard during the experiment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            //if we pressed space, move to next trial
            if (e.Key == Key.Space)
                mainExperiment.NextTrial();
        }

        /// <summary>
        /// Method that stops the main experiment and shuts down the system when the user closes the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            mainExperiment.StopExperiment();
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