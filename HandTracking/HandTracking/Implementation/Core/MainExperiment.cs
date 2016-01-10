﻿using System;
using System.Diagnostics;

using System.Threading;
using HandTracking.Implementation.HandTracking;
using HandTracking.Implementation.MarkerTracking;
using HandTracking.Interfaces.AudioController;
using HandTracking.Interfaces.Core;
using HandTracking.Interfaces.Module;
using HandTracking.Interfaces.Settings;

namespace HandTracking.Implementation.Core
{
    /// <summary>
    /// 
    /// </summary>
    class MainExperiment: IExperiment
    {

        #region main variables

        private readonly ICondition[] _conditions;
        private ISpeakerController _speakerController;
        private readonly Thread _experimentThread;
        private Thread _processingThread;

        //other vars
        private readonly Stopwatch _stopwatch;

        //we lock the thread on this object
        private readonly AutoResetEvent _resetEvent;

        private volatile bool _experimentIsRunning;
        private volatile bool _isProcessing;

        #endregion

        #region tracking variables

        private HandTrackingModule _handTrackingModule;
        private MarkerTrackingModule _markerTrackingModule;
        private ITracking _markerTracking;
        private ITracking _handtracking;
        private HandTrackingData _handData;
        private MarkerData _markerData;

        #endregion

        #region settings vars

        private CameraSettings cameraSettings;
        private const int DEFAULT_RESOLUTION_WIDTH = 640;
        private const int DEFAULT_RESOLUTION_HEIGHT = 480;
        private const int DEFAULT_FPS = 30;

        #endregion

        /// <summary>
        /// Constructor for main experiment.
        /// </summary>
        /// <param name="conditions">Conditions for the experiment, as array</param>
        /// <param name="speakerController"></param>
        public MainExperiment(ICondition[] conditions, ISpeakerController speakerController)
        {
            //set main experiment variables
            _conditions = conditions;
            _speakerController = speakerController;
            _experimentThread = new Thread(MainExperimentThread);
            _stopwatch = new Stopwatch();
            _resetEvent = new AutoResetEvent(false);

            //speaker settings
            _speakerController = speakerController;

            //camera settings
            cameraSettings = new CameraSettings(DEFAULT_RESOLUTION_WIDTH, DEFAULT_RESOLUTION_HEIGHT, DEFAULT_FPS);

            //initialize modules
            InitializeModules();
        }

        private void InitializeModules()
        {
            //create instance of marker tracking module
            _markerTrackingModule = new MarkerTrackingModule();
            _markerTracking = _markerTrackingModule.GetInstance(cameraSettings);
            _markerData = _markerTracking.GetData() as MarkerData;

            //create an instance of hand tracking module
            _handTrackingModule = new HandTrackingModule();
            _handtracking = _handTrackingModule.GetInstance();
            _handData = _handtracking.GetData() as HandTrackingData;

            //start variables
            _experimentIsRunning = true;
            _isProcessing = true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void MainExperimentThread()
        {
            //start the hand tracking module
            _handtracking.StartProcessing();

            //start the marker tracking module
            _markerTracking.StartProcessing();

            ///////////////////////////////////////////////////////////////////////////////////////////////////

            //testing
//            _speakerController.PlaySounds();

            /////////////////////////////////////////////////////////////////////////////////////////////////////

            //get number of speakers - excluding wrist
            int numberOfSpeakers = _speakerController.GetNumberOfSpeakers();

            Console.WriteLine(@"Experiment started.");
            foreach (var cond in _conditions)
            {

                //get the number of trials
                int trials = cond.GetNumberOfTrials();

                Console.WriteLine(@"Condition started");
                for(int i = 0; i < numberOfSpeakers * trials; i++)
                {
                    Console.WriteLine( @"Trial + " + i + @" started. Press space to move to next trial");
   
                    //start stopwatch
                    _stopwatch.Start();

                    //start the processing thread
                    _isProcessing = true;
                    _processingThread = new Thread(ProcessingThread);
                    _processingThread.Start();

                    //wait for main thread to signal key pressed
                    _resetEvent.WaitOne();

                    //stop watch and processing thread
                    _stopwatch.Stop();
                    _isProcessing = false;

                    Console.WriteLine(@"Space pressed. Time taken: " + _stopwatch.Elapsed + @" Moving to next trial.");
                     
                    //reset stopwatch
                    _stopwatch.Reset();

                    if (!_experimentIsRunning)
                        return;
                }

                Console.WriteLine(@"Condition ended.");
                if (!_experimentIsRunning)
                    return;
            }

            Console.WriteLine(@"Experiment ended.");
            StopExperiment();
        }

        private void ProcessingThread()
        {
            while (_isProcessing)
            {
                PXCMPoint3DF32 handPosition = _handData.getHandPosition3D();
                /*
                Console.WriteLine(@"\nProcessing Thread: ");
                Console.Write(@"\nX: " + handPosition.x);
                Console.Write(@"\nY: " + handPosition.y);
                Console.Write(@"\nZ: " + handPosition.z);
                */
//              Console.WriteLine(@"We are processing....");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// TODO: must implement return values for each thread state
        public void StartExperiment()
        {
            if (_experimentThread.IsAlive)
                return;

            _experimentThread.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// TODO: must implement return values for each thread state
        public void PauseExperiment()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// TODO: must implement return values for each thread state
        public void StopExperiment()
        {
            //stop hand tracking thread
            _handtracking.StopProcessing();

            //stop processing thread
            _isProcessing = false;

            //stop main experiment thread
            _experimentIsRunning = false;

            //move to next trial in case we are waiting for key pressed
            NextTrial();
        }

        /// <summary>
        /// An event which starts up the next trial.
        /// </summary>
        public void NextTrial()
        {
            _resetEvent.Set();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsStarted()
        {
            return _experimentThread.IsAlive;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsStopped()
        {
            return !_experimentThread.IsAlive;
        }
    }
}
