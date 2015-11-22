using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HandTracking.Implementation.HandTracking;
using HandTracking.Interfaces.Controller;
using HandTracking.Interfaces.Core;
using HandTracking.Interfaces.Module;

namespace HandTracking.Implementation.Core
{
    /// <summary>
    /// 
    /// </summary>
    class MainExperiment: IExperiment
    {

        #region main variables

        private ICondition[] _conditions;
        private ISpeakerController _speakerController;
        private Thread experimentThread;
        private Thread processingThread;

        //other vars
        private Stopwatch stopwatch;

        //we lock the thread on this object
        private AutoResetEvent resetEvent;

        private volatile bool experimentIsRunning;
        private volatile bool isProcessing;

        #endregion

        #region tracking variables

        private HandTrackingModule _handTrackingModule;
        private ITracking _tracking;
        private TrackingData _handData;

        #endregion

        /// <summary>
        /// Constructor for main experiment.
        /// </summary>
        /// <param name="conditions"></param>
        public MainExperiment(ICondition[] conditions)
        {
            //set main experiment variables
            _conditions = conditions;
            experimentThread = new Thread(MainExperimentThread);
            stopwatch = new Stopwatch();
            resetEvent = new AutoResetEvent(false);

            //initialize modules
            initializeModules();
        }

        private void initializeModules()
        {
            //create an instance of hand tracking module
            _handTrackingModule = new HandTrackingModule();
            //retrieve processing thread
            _tracking = _handTrackingModule.GetInstace();
            //get data interface for processing thread
            _handData = _tracking.GetHandData() as TrackingData;

            //start variables
            experimentIsRunning = true;
            isProcessing = true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void MainExperimentThread()
        {
            //start the hand tracking module
            _tracking.StartProcessing();

            Console.WriteLine(@"Experiment started.");
            foreach (ICondition cond in _conditions)
            {
                Console.WriteLine(@"Condition started");
                ITrial[] trials = cond.Trials;
                foreach (ITrial trial in trials)
                {
                    Console.WriteLine(trial.ToString() + @" started. Press space to move to next trial");

                    //start stopwatch
                    stopwatch.Start();

                    //start the processing thread
                    isProcessing = true;
                    processingThread = new Thread(ProcessingThread);
                    processingThread.Start();

                    //wait for main thread to signal key pressed
                    resetEvent.WaitOne();

                    //stop watch and processing thread
                    stopwatch.Stop();
                    isProcessing = false;

                    Console.WriteLine(@"Space pressed. Time taken: " + stopwatch.Elapsed + " Moving to next trial.");
                     
                    //reset stopwatch
                    stopwatch.Reset();

                    if (!experimentIsRunning)
                        return;
                }

                Console.WriteLine(@"Condition ended.");
                if (!experimentIsRunning)
                    return;
            }

            Console.WriteLine(@"Experiment ended.");
        }

        private void ProcessingThread()
        {
            while (isProcessing)
            {
                PXCMPoint3DF32 handPosition = _handData.getHandPosition3D();
                Console.WriteLine("\nProcessing Thread: ");
                Console.Write("X: " + handPosition.x);
                Console.Write("\nY: " + handPosition.y);
                Console.Write("\nZ: " + handPosition.z);
//                Console.WriteLine(@"We are processing....");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// TODO: must implement return values for each thread state
        public void StartExperiment()
        {
            if (experimentThread.IsAlive)
                return;

            experimentThread.Start();
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
            _tracking.StopProcessing();

            //stop processing thread
            isProcessing = false;

            //stop main experiment thread
            experimentIsRunning = false;

            //move to next trial in case we are waiting for key pressed
            NextTrial();
        }

        /// <summary>
        /// An event which starts up the next trial.
        /// </summary>
        public void NextTrial()
        {
            resetEvent.Set();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsStarted()
        {
            return experimentThread.IsAlive;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsStopped()
        {
            return !experimentThread.IsAlive;
        }
    }
}
