using System;
using System.Diagnostics;
using System.Threading;
using HandTracking.Implementation.HandTracking;
using HandTracking.Interfaces.AudioController;
using HandTracking.Interfaces.Core;
using HandTracking.Interfaces.Module;
using HandTracking.Interfaces.Settings;

namespace HandTracking.Implementation.Core
{
    /// <summary>
    /// </summary>
    internal class MainExperiment : IExperiment
    {
        /// <summary>
        ///     Constructor for main experiment.
        /// </summary>
        /// <param name="conditions">Conditions for the experiment, as array</param>
        /// <param name="speakerController"></param>
        public MainExperiment(ConditionImpl[] conditions, SpeakerController speakerController)
        {
            //set main experiment variables
            _conditions = conditions;
            _speakerController = speakerController;
            _experimentThread = new Thread(MainExperimentThread);
            _stopwatch = new Stopwatch();
            _resetEvent = new AutoResetEvent(false);

            //camera settings
            cameraSettings = new CameraSettings(DEFAULT_RESOLUTION_WIDTH, DEFAULT_RESOLUTION_HEIGHT, DEFAULT_FPS);

            //initialize modules
            InitializeModules();
        }

        /// <summary>
        /// </summary>
        /// TODO: must implement return values for each thread state
        public void StartExperiment()
        {
            if (_experimentThread.IsAlive)
                return;

            _experimentThread.Start();
        }

        /// <summary>
        /// </summary>
        /// TODO: must implement return values for each thread state
        public void PauseExperiment()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// TODO: must implement return values for each thread state
        public void StopExperiment()
        {
            //stop hand tracking thread
            _handtracking?.StopProcessing();
            _handtracking = null;

            //stop processing thread
            _isProcessing = false;

            //stop main experiment thread
            _experimentIsRunning = false;

            //move to next trial in case we are waiting for key pressed
            NextTrial();
        }

        /// <summary>
        ///     An event which starts up the next trial.
        /// </summary>
        public void NextTrial()
        {
            _resetEvent.Set();
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public bool IsStarted()
        {
            return _experimentThread.IsAlive;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public bool IsStopped()
        {
            return !_experimentThread.IsAlive;
        }

        /// <summary>
        ///     Method that initializes the modules of the main experiment
        /// </summary>
        private void InitializeModules()
        {
            //create instance of marker tracking module


            //create an instance of hand tracking module
            _handTrackingModule = new HandTrackingModule();
            _handtracking = _handTrackingModule.GetInstance();
            _handtracking.InitializeCameraModules();
            _handData = _handtracking.GetData() as HandTrackingData;

            //start variables
            _experimentIsRunning = true;
            _isProcessing = true;
        }

        /// <summary>
        /// </summary>
        private void MainExperimentThread()
        {
            //start the hand tracking module
            _handtracking.StartProcessing();

            //get number of speakers - excluding wrist
            int numberOfSpeakers = _speakerController.GetNumberOfSpeakers();

            Console.WriteLine(@"Experiment started.");
            foreach (var cond in _conditions)
            {
                //get the number of trials
                int trials = cond.NumberOfTrials;

                //set the audio design on the speaker controller
                _speakerController.SetAudioDesign(cond.AudioDesign);

                Console.WriteLine(@"Condition started");
                for (var i = 0; i < trials; i++)
                {
                    Console.WriteLine(@"Trial + " + i + @" started.");
                    for (var j = 0; j < numberOfSpeakers; j++)
                    {
                        //signal speaker controller to move to next speaker
                        _speakerController.NextSpeaker();

                        Console.WriteLine(@"Press space to move to next speaker");

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

                    //signal speaker controller to re-shuffle speaker flags
                    _speakerController.SignalConditionEnded(true);

                }

                Console.WriteLine(@"Condition ended.");
                if (!_experimentIsRunning)
                    return;

                //signal speaker controller to re-shuffle speaker flags
                _speakerController.SignalConditionEnded(true);
            }

            Console.WriteLine(@"Experiment ended.");
            StopExperiment();
        }

        private void ProcessingThread()
        {
            //selected speaker location
            //TODO: get position of target speaker

            while (_isProcessing)
            {
                var handPosition = _handData.Location3D;

                //TODO: calculate distance between hand and speaker

                //TODO: pass it to speaker controller

                //TODO: play audio feedback - pass the distance 
                _speakerController.PlaySounds(0);
            }
        }

        /// <summary>
        ///     Method that returns the distance between two points in 3D space. The two points must
        ///     be measured in the same units. (e.g. either meters or millimeters)
        /// </summary>
        /// <param name="point1">First point</param>
        /// <param name="point2">Second point</param>
        /// <returns>The distance in the unit of measurement between the two points</returns>
        private double GetDistance(PXCMPoint3DF32 point1, PXCMPoint3DF32 point2)
        {
            return
                Math.Sqrt(Math.Pow(point1.x - point2.x, 2) + Math.Pow(point1.y - point2.y, 2) +
                          Math.Pow(point1.z - point2.z, 2));
        }

        #region main variables

        private readonly ConditionImpl[] _conditions;
        private SpeakerController _speakerController;
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

        private Tracking _handtracking;
        private HandTrackingData _handData;

        #endregion

        #region settings vars

        private CameraSettings cameraSettings;
        private const int DEFAULT_RESOLUTION_WIDTH = 640;
        private const int DEFAULT_RESOLUTION_HEIGHT = 480;
        private const int DEFAULT_FPS = 30;

        #endregion
    }
}