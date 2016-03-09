using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using AudioModule.Implementation.AudioController;
using AudioModule.Interfaces;
using CameraModule.Implementation.HandTracking;
using CameraModule.Interfaces.Module;
using CoreModule.Implementation.Data;
using CoreModule.Interfaces;

namespace CoreModule.Implementation
{
    /// <summary>
    /// </summary>
    public class MainExperiment : IExperiment
    {
        /// <summary>
        ///     Constructor for main experiment.
        /// </summary>
        public MainExperiment()
        {
            InitializeExperiment();
        }

        /// <summary>
        /// </summary>
        public void StartExperiment()
        {
            InitializeExperiment();

            if (_experimentThread.IsAlive)
                return;

            //start variablesE
            _experimentIsRunning = true;
            _isProcessing = true;

            //start experiment
            _experimentThread.Start();
        }

        /// <summary>
        /// </summary>
        /// TODO: implement pause
        public void PauseExperiment()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        public void StopExperiment()
        {
            //stop hand tracking thread
            _handtracking.StopProcessing();
            _handtracking = null;

            //stop playback
            _speakerController.StopPlayback();

            //stop processing thread
            _isProcessing = false;

            //stop main experiment thread
            _experimentIsRunning = false;

//            _experimentThread?.Abort();

            //move to next trial in case we are waiting for key pressed
            NextTrial();
            _experimentThread.Join();

            _dataExporter.CloseStream();
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
        ///     Method that sets the current participant of the experiment.
        /// </summary>
        /// <param name="participant"></param>
        public void SetParticipant(Participant participant)
        {
            if (participant == null)
                throw new ExperimentException("Participant cannot be null");

            if (!IsStarted())
                _participant = participant;
        }

        /// <summary>
        ///     Method which initializes a new experiment.
        /// </summary>
        private void InitializeExperiment()
        {
            _experimentThread = new Thread(MainExperimentThread);
            _stopwatch = new Stopwatch();
            _resetEvent = new AutoResetEvent(false);
        }

        /// <summary>
        ///     Method that initializes the modules of the main experiment
        /// </summary>
        public void SetExperimentData(List<ConditionGroup> conditionGroups, SpeakerControllerImpl speakerController,
            HandTrackingImpl handTracking)
        {
            //set condition groups
            _conditionGroups = conditionGroups;

            //set the speaker controller
            _speakerController = speakerController;

            // initialize hand tracking
            _handtracking = handTracking;
            _handtracking.InitializeCameraModules();
            _handData = _handtracking.GetData() as HandTrackingData;
        }

        /// <summary>
        ///     Method that executes the main experiment thread and is run by pressing the Start button on the
        ///     UI. If the speakers and hand tracking have been successfully instantiated, then the experiment starts.
        ///     The method goes through all condition groups and, at each condition, it injects the audio design of the
        ///     condition to the Speaker Controller which will output feedback based on that particular audio design.
        /// </summary>
        private void MainExperimentThread()
        {
            //start the hand tracking module
            _handtracking.StartProcessing();

            //get number of speakers - excluding wrist
            var numberOfSpeakers = _speakerController.GetNumberOfSpeakers();

            Console.WriteLine(@"Experiment started.");
            foreach (var group in _conditionGroups)
            {
                foreach (var cond in group.Conditions)
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

                            //clean up
                            CleanUpTrial();

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
                }
            }

            CleanUpExperiment();
            Console.WriteLine(@"Experiment ended.");
        }

        /// <summary>
        ///     Method that cleans up the experiment after it has successfuly completed without user intervention.
        /// </summary>
        private void CleanUpExperiment()
        {
            _dataExporter.CloseStream();

            //stop hand tracking thread
            _handtracking.StopProcessing();
            _handtracking = null;

            //stop processing thread
            _isProcessing = false;

            //stop main experiment thread
            _experimentIsRunning = false;
        }

        /// <summary>
        ///     Method that cleans up after the participant has pressed Space.
        /// </summary>
        private void CleanUpTrial()
        {

            //stop watch and processing thread
            _stopwatch.Stop();
            _isProcessing = false;

            //get time elapsed
            var time = (long) _stopwatch.Elapsed.TotalMilliseconds;

//            Console.WriteLine(@"Space pressed. Time taken: " + time +
//                              @" Moving to next trial.");

            //reset stopwatch
            _stopwatch.Reset();

            //stop current playback and play confirm sound
            _speakerController.StopSounds();
            _speakerController.PlayConfirm();

            //add data
            var speakerPosition = _speakerController.GetSpeakerPosition();
            var distance = SpeakerController.GetDistance(_handData.Location3D, speakerPosition);
            var closest = _speakerController.GetClosest(_handData.Location3D);

            //export data to file
            _dataExporter.SetTrialData(_speakerController.GetSpeakerId(), closest, distance, time,
                _handData.Location3D);

            //reset hand detected
            _handData.ResetHand();

            //wait 2 seconds before proceeding
            Thread.Sleep(2000);
        }

        private void ProcessingThread()
        {
            //TODO: catch exception
            _speakerController.PlaySounds();

            while (_isProcessing)
            {
                //if hand is not detected, set the distance to maximum
                if (!_handData.HandDetected)
                {
                    _speakerController.SetDistance(50);
                    continue;
                }

                var handPosition = _handData.Location3D;

//                Console.WriteLine("Hand position X:" + handPosition.x + " Y:" + handPosition.y + " Z:" + handPosition.z);

                //get speaker position
                var speakerPosition = _speakerController.GetSpeakerPosition();

//                Console.WriteLine("Speaker X:" + speakerPosition.x + " Y:" + speakerPosition.y + " Z:" + speakerPosition.z);

                //calculate distance between hand and speaker
                var distance = SpeakerController.GetDistance(handPosition, speakerPosition);

//                Console.WriteLine(@"Distance: " + distance);

                //pass distance to speaker controller
                _speakerController.SetDistance(distance);
            }
        }

        #region main variables

        private List<ConditionGroup> _conditionGroups;
        private SpeakerControllerImpl _speakerController;
        private Thread _experimentThread;
        private Thread _processingThread;
        private DataExporter _dataExporter;
        private Participant _participant;

        //other vars
        private Stopwatch _stopwatch;

        //we lock the thread on this object
        private AutoResetEvent _resetEvent;

        private volatile bool _experimentIsRunning;
        private volatile bool _isProcessing;

        #endregion

        #region tracking variables

        private Tracking _handtracking;
        private HandTrackingData _handData;

        #endregion
    }
}