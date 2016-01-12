using System;
using System.Threading;
using HandTracking.Interfaces.Module;
using HandTracking.Interfaces.Settings;

namespace HandTracking.Implementation.HandTracking
{
    internal class HandTrackingImpl : Tracking
    {
        /// <summary>
        ///     Create a new HandTrackingImpl instance with default settings.
        /// </summary>
        protected internal HandTrackingImpl()
        {
//            Settings = new HandTrackingSettings();
//            Data = new HandTrackingData();
            _handTrackingSettings = new HandTrackingSettings();
            _handTrackingData = new HandTrackingData();
        }

        /// <summary>
        ///     Create a new HandTrackingImpl instance with custom settings.
        /// </summary>
        /// <param name="settings"></param>
        protected internal HandTrackingImpl(ISettings settings)
        {
            _handTrackingSettings = (HandTrackingSettings) settings;
            _handTrackingData = new HandTrackingData();
        }

        /// <summary>
        ///     Met  od that starts the processing thread.
        /// </summary>
        public override void StartProcessing()
        {
            if (_isInitialized)
            {
                _processingThread = new Thread(HandTrackingThread);
                _isProcessing = true;
                _processingThread.Start();
            }
            else throw new HandTrackingNotInitializedException("Hand tracking RealSense modules have not been initialized.");
        }

        public override void StopProcessing()
        {
            //terminate thread
            _isProcessing = false;

            _processingThread = null;
        }

        public override void PauseProcessing()
        {
            throw new NotImplementedException();
        }

        public override IData GetData()
        {
            return _handTrackingData;
        }

        /// <summary>
        ///     Method that initializes camera modules.
        /// </summary>
        public override bool InitializeCameraModules()
        {
            _session = PXCMSession.CreateInstance();

            _senseManager = _session.CreateSenseManager();

            if (_senseManager == null)
            {
                Console.WriteLine(@"Failed to create the SenseManager object.");
                return false;
            }

            // Enabling the Hand module
            var enablingModuleStatus = _senseManager.EnableHand("Hand Module");

            if (enablingModuleStatus != pxcmStatus.PXCM_STATUS_NO_ERROR)
            {
                Console.WriteLine(@"Failed to enable the Hand Module");
                return false;
            }

            //getting instance of hand module
            _handModule = _senseManager.QueryHand();

            if (_handModule == null)
            {
                Console.WriteLine(@"Failed to get the HandModule object.");
                return false;
            }

            //create hand configuration
            _handConfiguration = _handModule.CreateActiveConfiguration();

            if (_handConfiguration == null)
            {
                Console.WriteLine(@"Failed to create the HandConfiguration object.");
                return false;
            }

            //apply settings
            if (_handTrackingSettings != null)
            {
                _handConfiguration.EnableStabilizer(_handTrackingSettings.EnableStabilizer);
                _handConfiguration.SetSmoothingValue(_handTrackingSettings.SmoothingValue);
            }

            _handConfiguration.ApplyChanges();

            // Creating a data output object
            _handData = _handModule.CreateOutput();

            if (_handData == null)
            {
                Console.WriteLine(@"Failed to create the HandData object.");
                return false;
            }

            // Initializing the SenseManager
            if (_senseManager.Init() != pxcmStatus.PXCM_STATUS_NO_ERROR)
            {
                Console.WriteLine(@"Failed to initialize the SenseManager");
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Method that is run in a separate thread.
        /// </summary>
        private void HandTrackingThread()
        {
            // Looping to query the hands information
            while (_isProcessing)
            {
                // Acquiring a frame
                if (_senseManager.AcquireFrame(true) < pxcmStatus.PXCM_STATUS_NO_ERROR)
                {
                    break;
                }

                // Updating the hand data
                _handData?.Update();

                // Processing Hands
                ProcessHands(_handData);

                // Releasing the acquired frame
                _senseManager.ReleaseFrame();
            }

            // Releasing resources
            _handData?.Dispose();
            _handConfiguration?.Dispose();

            _senseManager.Close();
            _senseManager.Dispose();
            _session.Dispose();
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
            Console.WriteLine(@"{0} hand(s) were detected.", numberOfHands);

            // Querying the information about detected hands
            for (var i = 0; i < numberOfHands; i++)
            {
                // Querying hand id
                int handId;

                //closest hand gets smaller id
                var queryHandIdStatus = handData.QueryHandId(PXCMHandData.AccessOrderType.ACCESS_ORDER_NEAR_TO_FAR, i,
                    out handId);

                //check for errors
                if (queryHandIdStatus != pxcmStatus.PXCM_STATUS_NO_ERROR)
                {
                    Console.WriteLine(@"Failed to query the hand Id.");
                    continue;
                }

                Console.WriteLine(@"Hand id: {0}", handId);

                // Querying the hand data
                PXCMHandData.IHand hand;
                var queryHandStatus = handData.QueryHandDataById(handId, out hand);

                if (queryHandStatus == pxcmStatus.PXCM_STATUS_NO_ERROR && hand != null)
                {
                    // Querying Hand 2D Position
                    var massCenterImage = hand.QueryMassCenterImage();
                    Console.WriteLine(@"Hand position on image: {0} | {1}", massCenterImage.x, massCenterImage.y);

                    // Querying Hand 3D Position
                    var massCenterWorld = hand.QueryMassCenterWorld();
                    Console.WriteLine(@"Hand position on world: {0} | {1} | {2}", massCenterWorld.x, massCenterWorld.y,
                        massCenterWorld.z);

                    // Querying Hand Joints
                    if (hand.HasTrackedJoints())
                    {
                        //searching for location of center hand
                        var jointType = PXCMHandData.JointType.JOINT_CENTER;
                        PXCMHandData.JointData jointData;
                        var queryStatus = hand.QueryTrackedJoint(jointType, out jointData);

                        if (queryStatus == pxcmStatus.PXCM_STATUS_NO_ERROR && jointData != null)
                        {
//                            // Printing the 2D position (image)
//                            Console.WriteLine(@"	2D Position: {0} | {1}", jointData.positionImage.x,
//                                jointData.positionImage.y);

                            //set 2D position in hand Data
                            _handTrackingData.Location2D = jointData.positionImage;

//                            // Printing the 3D position (depth)
//                            Console.WriteLine(@"	3D Position: {0} | {1} | {2}", jointData.positionWorld.x,
//                                jointData.positionWorld.y, jointData.positionWorld.z);

                            //set 3D position in hand Data
                            _handTrackingData.Location3D = jointData.positionWorld;
                        }
                    }
                }

                Console.WriteLine(@"----------");
            }
        }

        #region Module vars

        private HandTrackingData _handTrackingData;
        private readonly HandTrackingSettings _handTrackingSettings;

        #endregion

        #region RealSense vars

        private PXCMSession _session;
        private PXCMSenseManager _senseManager;
        private PXCMHandModule _handModule;
        private PXCMHandConfiguration _handConfiguration;
        private PXCMHandData _handData;

        #endregion

        #region threading vars

        private bool _isProcessing;
        private readonly bool _isInitialized = true;
        private Thread _processingThread;

        #endregion
    }
}