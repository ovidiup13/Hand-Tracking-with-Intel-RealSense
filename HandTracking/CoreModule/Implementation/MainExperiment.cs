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
        #region experiment controls

        /// <summary>
        ///     Constructor for main experiment.
        /// </summary>
        public MainExperiment()
        {
            ExperimentStatus = ExperimentStatus.Stopped;
        }

        /// <summary>
        ///     Method that starts the current experiment.
        /// </summary>
        public void StartExperiment()
        {
            //initialize experiment if not started or stopped
            if (ExperimentStatus != ExperimentStatus.Stopped) return;

            InitializeExperiment();
            //set processing flag and start the thread
            _isProcessing = true;
            _experimentThread.Start();

            //start variables
            ExperimentStatus = ExperimentStatus.Running;
        }

        /// <summary>
        ///     Method that pauses the experiment. Sets the waitForPause event to 1 and the main experiment thread will
        ///     pause until it is set to 0.
        /// </summary>
        public void PauseExperiment()
        {
            if (ExperimentStatus == ExperimentStatus.Running)
            {
                _pauseFlag = true;
            }
        }

        /// <summary>
        /// </summary>
        public void StopExperiment()
        {          
            //move to next trial in case we are waiting for key pressed
            _experimentThread?.Abort();
            _experimentThread?.Join();

            ExperimentStatus = ExperimentStatus.Stopped;
        }

        /// <summary>
        ///     Method that resumes the experiment thread and sets the Status to Running. If the experiment is not in Paused
        ///     status,
        ///     the call will be ignored.
        /// </summary>
        public void ResumeExperiment()
        {
            if (ExperimentStatus != ExperimentStatus.Paused) return;
            
            //resume
            _waitForPauseEvent.Set();
            ExperimentStatus = ExperimentStatus.Running;
            
        }

        /// <summary>
        ///     An event which starts up the next trial. The next trial will only be available after the speaker controller
        ///     has initialized audio feedback.
        /// </summary>
        public void NextTrial()
        {
            if (_speakerController.IsPlaying)
                _waitForSpaceEvent.Set();
        }

        /// <summary>
        ///     Method that sets the current participant of the experiment.
        /// </summary>
        /// <param name="participant"></param>
        public void SetParticipant(Participant participant)
        {
            if (participant == null)
            {
                throw new ExperimentException("Participant cannot be null");
            }

            if (ExperimentStatus != ExperimentStatus.Stopped)
            {
                throw new ExperimentException("Cannot set a new participant. Experiment is currently running.");
            }

            _participant = participant;
        }

        /// <summary>
        ///     Method which initializes a new experiment.
        /// </summary>
        private void InitializeExperiment()
        {
            //initialize thread and stopwatch
            _experimentThread = new Thread(MainExperimentThread);
            _stopwatch = new Stopwatch();

            //events
            _waitForSpaceEvent = new AutoResetEvent(false);
            _waitForPauseEvent = new AutoResetEvent(false);
            _pauseFlag = false;

            //init data exporter
            _dataExporter = new DataExporter(_participant);

            //set status
            ExperimentStatus = ExperimentStatus.Initialized;
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

        #endregion

        #region inner working of the experiment

        /// <summary>
        ///     Method that executes the main experiment thread and is run by pressing the Start button on the
        ///     UI. If the speakers and hand tracking have been successfully instantiated, then the experiment starts.
        ///     The method goes through all condition groups and, at each condition, it injects the audio design of the
        ///     condition to the Speaker Controller which will output feedback based on that particular audio design.
        /// </summary>
        private void MainExperimentThread()
        {
            try
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

                        _dataExporter.CreateConditionFile(cond.AudioDesign.ToString());

                        //set the audio design on the speaker controller
                        _speakerController.SetAudioDesign(cond.AudioDesign);

                        Console.WriteLine(@"Condition started");
                        for (var i = 0; i < trials; i++)
                        {
                            Console.WriteLine(@"Trial + " + i + @" started.");
                            Console.WriteLine(@"Press space to move to next speaker");
                            for (var j = 0; j < numberOfSpeakers; j++)
                            {
                                //signal speaker controller to move to next speaker
                                _speakerController.NextSpeaker();
                                _dataExporter.CreateMovementTraceFile(cond.AudioDesign.ToString(), _speakerController.GetSpeakerId());

                                //start stopwatch
                                _stopwatch.Start();

                                //start the processing thread
                                _isProcessing = true;
                                _processingThread = new Thread(ProcessingThread);
                                _processingThread.Start();

                                //wait for main thread to signal key pressed
                                _waitForSpaceEvent.WaitOne();

                                //clean up
                                CleanUpTrial();
                                _dataExporter.CloseTraceStream();
                            }

                            //signal speaker controller to re-shuffle speaker flags
                            _speakerController.SignalTrialEnded(true);
                        }

                        Console.WriteLine(@"Condition ended.");

                        //signal speaker controller to re-shuffle speaker flags for new condition
                        _speakerController.SignalConditionEnded(true);
                        _dataExporter.CloseConditionStream();
                    }
                }
            }
            catch (ThreadAbortException abortException)
            {
                Console.WriteLine("Main Experiment has been aborted by the user.");
            }
            finally
            {
                CleanUpExperiment();
                Console.WriteLine(@"Experiment ended.");
            }

        }

        /// <summary>
        ///     Method that cleans up the experiment after it has successfuly completed without user intervention.
        /// </summary>
        private void CleanUpExperiment()
        {
            //close the data stream
            _dataExporter.CloseStreams();

            //stop processing thread
            _isProcessing = false;
            _processingThread.Abort();
            _processingThread.Join();

            //stop playback
            _speakerController.StopPlayback();

            //stop hand tracking thread
            _handtracking.StopProcessing();
            _handtracking = null;

            ExperimentStatus = ExperimentStatus.Stopped;
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

            //reset stopwatch
            _stopwatch.Reset();

            //stop current playback and play confirm sound
            _speakerController.PlayConfirm();
            _speakerController.StopSounds();

            //add data
            var speakerPosition = _speakerController.GetSpeakerPosition();
            var distance = SpeakerController.GetDistance(_handData.Location3D, speakerPosition);
            var closest = _speakerController.GetClosest(_handData.Location3D);

            //export data to file
            _dataExporter.AppendToConditionFile(_speakerController.GetSpeakerId(), closest, distance, time,
                _handData.Location3D);

            //reset hand detected
            _handData.ResetHand();

            //check if experimenter wants to pause
            CheckIfPaused();

            //wait 2 seconds before proceeding
            Thread.Sleep(2000);
        }

        /// <summary>
        ///     Method that checks whether the experiment Pause flag has been raised. If so,
        ///     it will stop
        /// </summary>
        private void CheckIfPaused()
        {
            //check if paused
            if (!_pauseFlag) return;

            Console.WriteLine("Experiment should be paused");
            ExperimentStatus = ExperimentStatus.Paused;
            _handtracking.StopProcessing();

            _waitForPauseEvent.WaitOne();

            _handtracking.InitializeCameraModules();
            _handtracking.StartProcessing();
            ExperimentStatus = ExperimentStatus.Running;
            _pauseFlag = false;
        }

        /// <summary>
        ///     Method run in a separate thread. It queries the hand and speaker controllers checking for new data.
        ///     The hand data is passed to the speaker controller which signals the AudioDesign to change its feedback
        ///     appropriately.
        ///     The thread runs until the participant presses space on the keyboard.
        /// </summary>
        private void ProcessingThread()
        {
            try
            {
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
                    _dataExporter.AppendToTraceFile(_stopwatch.ElapsedMilliseconds, handPosition);

                    //get speaker position
                    var speakerPosition = _speakerController.GetSpeakerPosition();

                    //calculate distance between hand and speaker
                    var distance = SpeakerController.GetDistance(handPosition, speakerPosition);

                    //pass distance to speaker controller
                    _speakerController.SetDistance(distance);
                    Thread.Sleep(50);
                }
            }
            catch (ThreadAbortException abortException)
            {
                Console.WriteLine("Processing Thread Aborted.");
            }
        }

        #endregion

        #region main variables

        private List<ConditionGroup> _conditionGroups;
        private SpeakerControllerImpl _speakerController;
        private Thread _experimentThread;
        private Thread _processingThread;
        private DataExporter _dataExporter;
        private Participant _participant;

        //other vars
        private Stopwatch _stopwatch;

        public ExperimentStatus ExperimentStatus { get; internal set; }

        //we lock the thread on this object
        private AutoResetEvent _waitForSpaceEvent;
        private AutoResetEvent _waitForPauseEvent;

        private volatile bool _pauseFlag;
        private volatile bool _isProcessing;

        #endregion

        #region tracking variables

        private Tracking _handtracking;
        private HandTrackingData _handData;

        #endregion
    }
}