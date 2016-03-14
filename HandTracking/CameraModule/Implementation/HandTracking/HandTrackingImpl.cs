using System;
using System.Threading;
using CameraModule.Interfaces;
using CameraModule.Interfaces.Module;
using CameraModule.Interfaces.Settings;

namespace CameraModule.Implementation.HandTracking
{
    public class HandTrackingImpl : Tracking
    {
        /// <summary>
        ///     Create a new HandTrackingImpl instance with default settings.
        /// </summary>
        public HandTrackingImpl()
        {
//            Settings = new HandTrackingSettings();
//            Data = new HandTrackingData();
            Settings = new HandTrackingSettings() {TrackingStatus = TrackingStatus.Stopped};
            Data = new HandTrackingData();
        }

        /// <summary>
        ///     Create a new HandTrackingImpl instance with custom settings.
        /// </summary>
        /// <param name="settings"></param>
        protected internal HandTrackingImpl(ISettings settings)
        {
            Settings = (HandTrackingSettings) settings;
            Data = new HandTrackingData();
        }

        /// <summary>
        ///     Method that starts the HandTracking thread.
        /// </summary>
        public override void StartProcessing()
        {
            if (Settings.TrackingStatus == TrackingStatus.Initialized)
            {
                _isPaused = false;
                _waitForPauseEvent = new AutoResetEvent(false);

                ProcessingThread = new Thread(TrackingThread);
                ProcessingThread.Start();
                Settings.TrackingStatus = TrackingStatus.Running;
            }
            else throw new HandTrackingException("Hand tracking RealSense modules have not been initialized.");
        }

        /// <summary>
        ///     Method that stops the HandTracking thread.
        /// </summary>
        public override void StopProcessing()
        {
            //terminate thread
            ProcessingFlag = false;
            ProcessingThread?.Abort();
            ProcessingThread?.Join();

            ProcessingThread = null;

            Settings.TrackingStatus = TrackingStatus.Stopped;
        }

        /// <summary>
        ///     Method that resumes the processing thread after the Paused flag has been set.
        /// </summary>
        public override void ResumeProcessing()
        {
            if (Settings.TrackingStatus == TrackingStatus.Paused)
            {
                _waitForPauseEvent.Set();
                _isPaused = false;
            }
            else throw new HandTrackingException("Hand tracking cannot be resumed since it's not in the Paused state.");
        }

        /// <summary>
        /// </summary>
        public override void PauseProcessing()
        {
            if (Settings.TrackingStatus == TrackingStatus.Running)
                _isPaused = true;
        }

        public override Data GetData()
        {
            return Data;
        }

        /// <summary>
        ///     Method that initializes camera modules.
        /// </summary>
        public override void InitializeCameraModules()
        {
            //init session
            Session = PXCMSession.CreateInstance();
            if (Session == null)
                throw new HandTrackingException(@"Failed to create Session");

            //create sense manager
            SenseManager = Session.CreateSenseManager();
            if (SenseManager == null)
                throw new HandTrackingException(@"Failed to create Sense Manager");

            // Enabling the Hand module
            var enablingModuleStatus = SenseManager.EnableHand("Hand Module");

            if (enablingModuleStatus != pxcmStatus.PXCM_STATUS_NO_ERROR)
                throw new HandTrackingException(@"Failed to enable the Hand Module");

            //initialize image and depth streams
            var imageStatus = SenseManager.EnableStream(PXCMCapture.StreamType.STREAM_TYPE_COLOR,
                Settings.Width,
                Settings.Height, Settings.FramesPerSecond);

            if (imageStatus != pxcmStatus.PXCM_STATUS_NO_ERROR)
                throw new HandTrackingException(@"Failed to enable Image Stream.");

            //getting instance of hand module
            _handModule = SenseManager.QueryHand();

            if (_handModule == null)
                throw new HandTrackingException(@"Failed to get the HandModule object.");

            //create hand configuration
            _handConfiguration = _handModule.CreateActiveConfiguration();

            if (_handConfiguration == null)
                throw new HandTrackingException(@"Failed to create the HandConfiguration object.");

            //apply settings
            if (Settings != null)
            {
                _handConfiguration.SetTrackingMode(Settings.TrackingModeType);
                _handConfiguration.EnableTrackedJoints(true);
                _handConfiguration.EnableStabilizer(Settings.EnableStabilizer);
                _handConfiguration.SetSmoothingValue(Settings.SmoothingValue);
            }

            _handConfiguration.ApplyChanges();

            // Creating a data output object
            _handData = _handModule.CreateOutput();

            if (_handData == null)
                throw new HandTrackingException(@"Failed to create the HandData object.");

            // Initializing the SenseManager
            if (SenseManager.Init() != pxcmStatus.PXCM_STATUS_NO_ERROR)
                throw new HandTrackingException(@"Failed to initialize the SenseManager");

            //set status to initialized
            Settings.TrackingStatus = TrackingStatus.Initialized;
        }

        /// <summary>
        ///     Method that is run in a separate thread.
        /// </summary>
        protected override void TrackingThread()
        {
            try
            {
                Console.WriteLine(@"Hand Tracking Started.");
                ProcessingFlag = true;
                // Looping to query the hands information
                while (ProcessingFlag)
                {
                    // Acquiring a frame
                    if (SenseManager.AcquireFrame(true) < pxcmStatus.PXCM_STATUS_NO_ERROR)
                    {
                        break;
                    }

                    //acquire color stream and pass it to the delegate
                    var sample = SenseManager.QuerySample();
                    ProcessImage(sample.color);

                    // Updating the hand data
                    _handData?.Update();

                    // Processing Hands
                    ProcessHands(_handData);

                    // Releasing the acquired frame
                    SenseManager.ReleaseFrame();
                }
            }
            catch (ThreadAbortException abortException)
            {
                Console.WriteLine("Hand Tracking Stopped.");
            }
            finally
            {
                // Releasing resources
                _handData?.Dispose();
                _handConfiguration?.Dispose();

                SenseManager.Close();
                SenseManager.Dispose();
                Session.Dispose();
                Console.WriteLine(@"Hand Tracking terminated.");
            }
        }

        private void ProcessImage(PXCMImage image)
        {
            PXCMImage.ImageData imageData;
            image.AcquireAccess(PXCMImage.Access.ACCESS_READ, PXCMImage.PixelFormat.PIXEL_FORMAT_RGB24, out imageData);

            //TODO: convert to bitmap in another thread
            // Converting the color image to System.Drawing.Bitmap
            var bitmap = imageData.ToBitmap(0, image.info.width, image.info.height);
            NewImageAvailable?.Invoke(this, new NewImageArgs(PXCMCapture.StreamType.STREAM_TYPE_COLOR, bitmap));

            //release access to image data
            image.ReleaseAccess(imageData);
        }

        /// <summary>
        ///     Method that processes hand data acquired from each frame.
        ///     Stores position in 2D and 3D space in IData object.
        /// </summary>
        /// <param name="handData"></param>
        private void ProcessHands(PXCMHandData handData)
        {
            // Querying how many hands were detected
            var numberOfHands = handData.QueryNumberOfHands();

            if (numberOfHands == 0)
            {
                Data.HandDetected = false;
//                Console.WriteLine(@"Hand not detected!");
                return;
            }

            //detected at least one hand
            Data.HandDetected = true;
            Console.WriteLine(@"Hand detected! Number of hands: " + numberOfHands);

            // Querying the information about detected hands
            for (var i = 0; i < numberOfHands; i++)
            {
                // Querying hand id
                int handId;

                //get hand data by order type
                var queryHandIdStatus = handData.QueryHandId(Settings.AccessOrderType, i,
                    out handId);

                //check for errors
                if (queryHandIdStatus != pxcmStatus.PXCM_STATUS_NO_ERROR)
                {
                    Console.WriteLine(@"Failed to query the hand Id.");
                    continue;
                }

                // Querying the hand data
                PXCMHandData.IHand hand;
                var queryHandStatus = handData.QueryHandDataById(handId, out hand);

                if (queryHandStatus != pxcmStatus.PXCM_STATUS_NO_ERROR || hand == null) continue;
                
                //extremities mode
                if (Settings.TrackingModeType == PXCMHandData.TrackingModeType.TRACKING_MODE_EXTREMITIES)
                {
                    //get extremity data
                    PXCMHandData.ExtremityData extremityData;
                    var extremityStatus = hand.QueryExtremityPoint(Settings.ExtremityType, out extremityData);

                    //check for errors
                    if (extremityStatus != pxcmStatus.PXCM_STATUS_NO_ERROR || extremityData == null)
                    {
                        Console.WriteLine("An error occurred while querying extremity data: " + extremityStatus);
                        continue;
                    }

                    //update hand location using extremity data
                    Data.Location2D = extremityData.pointImage;
                    var p = extremityData.pointWorld;
                    Data.Location3D = new PXCMPoint3DF32(p.x*Millis, p.y*Millis, p.z*Millis);
                }

                // full hand
                else if (hand.HasTrackedJoints())
                {
                    //get joint data
                    PXCMHandData.JointData jointData;
                    var queryStatus = hand.QueryTrackedJoint(Settings.JointType, out jointData);

                    //check for errors
                    if (queryStatus != pxcmStatus.PXCM_STATUS_NO_ERROR || jointData == null)
                    {
                        Console.WriteLine("An error occurred while querying joint data: " + queryStatus);
                        continue;
                    }

                    // update hand location using joint data
                    Data.Location2D = jointData.positionImage;
                    var p = jointData.positionWorld;
                    Data.Location3D = new PXCMPoint3DF32(p.x*Millis, p.y*Millis, p.z*Millis);
                }
            }
        }

        #region Module vars

        public event NewImageEventHandler NewImageAvailable;


        public new readonly HandTrackingData Data;
        public new HandTrackingSettings Settings { get; }

        private bool _isPaused;
        private AutoResetEvent _waitForPauseEvent;

        private const int Millis = 1000;

        #endregion

        #region RealSense vars

        private PXCMHandModule _handModule;
        private PXCMHandConfiguration _handConfiguration;
        private PXCMHandData _handData;

        #endregion
    }
}