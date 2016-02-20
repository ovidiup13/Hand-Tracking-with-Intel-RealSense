using System;
using System.Diagnostics;
using System.Threading;
using AudioModule.Interfaces;
using CameraModule.Interfaces.Module;
using CameraModule.Interfaces.Settings;
using CoreModule.Implementation.Data;
using CoreModule.Interfaces;
using HandModule.Implementation.HandTracking;

namespace CoreModule.Implementation
{
    /// <summary>   
    /// </summary>
    internal class MainExperiment : IExperiment
    {
        /// <summary>
        ///     Constructor for main experiment.
        /// </summary>
        /// <param name="conditions">Conditions for the experiment, as an array</param>
        /// <param name="speakerController">The speaker controller object</param>
        /// <param name="participant">The participant object for the experiment</param>
        public MainExperiment(ConditionImpl[] conditions, SpeakerController speakerController, Participant participant)
        {
            //set main experiment variables
            _conditions = conditions;
            _speakerController = speakerController;

            //check participant
            if (participant == null)
                throw new ArgumentNullException(nameof(participant));
            _participant = participant;

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
        /// <exception cref="ArgumentNullException">If the participant selected for the experiment is null.</exception>
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
        /// Method that sets the current participant of the experiment.
        /// </summary>
        /// <param name="participant"></param>
        public void SetParticipant(Participant participant)
        {
            if(!IsStarted())
                _participant = participant;
        }

        /// <summary>
        ///     Method that initializes the modules of the main experiment
        /// </summary>
        private void InitializeModules()
        {
            
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
            var numberOfSpeakers = _speakerController.GetNumberOfSpeakers();

            Console.WriteLine(@"Experiment started.");
            foreach (var cond in _conditions)
            {
                //get the number of trials
                var trials = cond.NumberOfTrials;

                //set the audio design on the speaker controller
                _speakerController.SetAudioDesign(cond.AudioDesign);
                _dataExporter = new DataExporter(_participant, cond.AudioDesign.ToString());

                Console.WriteLine(@"Condition started");
                for (var i = 0; i < trials; i++)
                {
                    Console.WriteLine(@"Trial + " + i + @" started.");
                    Console.WriteLine(@"Press space to move to next speaker");
                    for (var j = 0; j < numberOfSpeakers; j++)
                    {
                        //signal speaker controller to move to next speaker
                        _speakerController.NextSpeaker();

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

                        //get time elapsed
                        var time = (long) _stopwatch.Elapsed.TotalMilliseconds;

                        Console.WriteLine(@"Space pressed. Time taken: " + time +
                                          @" Moving to next trial.");

                        //reset stopwatch
                        _stopwatch.Reset();
                        _speakerController.PlayConfirm();

                        //add data
                        var speakerPosition = _speakerController.GetSpeakerPosition();
                        var distance = SpeakerController.GetDistance(_handData.Location3D, speakerPosition);
                        var closest = _speakerController.GetClosest(_handData.Location3D);

                        //export data to file
                        _dataExporter.SetTrialData(_speakerController.GetSpeakerId(), closest, distance, time, _handData.Location3D);

                        if (!_experimentIsRunning)
                            return;
                    }

                    //signal speaker controller to re-shuffle speaker flags
                    _speakerController.SignalTrialEnded(true);
                }

                Console.WriteLine(@"Condition ended.");

                if (!_experimentIsRunning)
                    return;

                //signal speaker controller to re-shuffle speaker flags for new condition
                _speakerController.SignalConditionEnded(true);

                //close condition stream
                _dataExporter.CloseStream();
            }

            Console.WriteLine(@"Experiment ended.");
            StopExperiment();
        }

        private void ProcessingThread()
        {
            //TODO: catch exception
            _speakerController.PlaySounds();

            double distance = 50;
            PXCMPoint3DF32 handPosition;
            while (_isProcessing)
            {

                //if hand is not detected, set the distance to maximum
                if (!_handData.HandDetected)
                {
                    _speakerController.SetDistance(50);
                    continue;
                }

                handPosition = _handData.Location3D;

//                Console.WriteLine("Hand position X:" + handPosition.x + " Y:" + handPosition.y + " Z:" + handPosition.z);

                //get speaker position
                var speakerPosition = _speakerController.GetSpeakerPosition();

//                Console.WriteLine("Speaker X:" + speakerPosition.x + " Y:" + speakerPosition.y + " Z:" + speakerPosition.z);
                    
                //calculate distance between hand and speaker
                distance = SpeakerController.GetDistance(handPosition, speakerPosition);

                Console.WriteLine(@"Distance: " + distance);

                //pass distance to speaker controller
                _speakerController.SetDistance(distance);
            }
        }

        

        #region main variables

        private readonly ConditionImpl[] _conditions;
        private readonly SpeakerController _speakerController;
        private readonly Thread _experimentThread;
        private Thread _processingThread;
        private DataExporter _dataExporter;
        private Participant _participant;

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