using System;
using System.Threading;
using HandTracking.Interfaces.Module;
using HandTracking.Interfaces.Settings;

namespace HandTracking.Implementation.HandTracking
{
    //TODO: replace all errors with exceptions
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
        /// Method that starts the HandTracking thread.
        /// </summary>
        public override void StartProcessing()
        {
            if (IsInitialized)
            {
                ProcessingThread = new Thread(TrackingThread);
                IsProcessing = true;
                ProcessingThread.Start();
            }
            else throw new HandTrackingException("Hand tracking RealSense modules have not been initialized.");
        }

        /// <summary>
        /// Method that stops the HandTracking thread.
        /// </summary>
        public override void StopProcessing()
        {
            //terminate thread
            ProcessingFlag = false;
            ProcessingThread = null;
            IsProcessing = false;
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

            //getting instance of hand module
            _handModule = SenseManager.QueryHand();

            if (_handModule == null)
                throw new HandTrackingException(@"Failed to get the HandModule object.");

            //create hand configuration
            _handConfiguration = _handModule.CreateActiveConfiguration();

            if (_handConfiguration == null)
                throw new HandTrackingException(@"Failed to create the HandConfiguration object.");

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
                throw new HandTrackingException(@"Failed to create the HandData object.");

            // Initializing the SenseManager
            if (SenseManager.Init() != pxcmStatus.PXCM_STATUS_NO_ERROR)
                throw new HandTrackingException(@"Failed to initialize the SenseManager");

            IsInitialized = true;
        }

        /// <summary>
        ///     Method that is run in a separate thread.
        /// </summary>
        protected override void TrackingThread()
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

                // Updating the hand data
                _handData?.Update();

                // Processing Hands
                ProcessHands(_handData);

                // Releasing the acquired frame
                SenseManager.ReleaseFrame();
            }

            // Releasing resources
            _handData?.Dispose();
            _handConfiguration?.Dispose();

            SenseManager.Close();
            SenseManager.Dispose();
            Session.Dispose();

            Console.WriteLine(@"Hand Tracking terminated.");
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
//            Console.WriteLine(@"{0} hand(s) were detected.", numberOfHands);

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

//                Console.WriteLine(@"Hand id: {0}", handId);

                // Querying the hand data
                PXCMHandData.IHand hand;
                var queryHandStatus = handData.QueryHandDataById(handId, out hand);

                if (queryHandStatus == pxcmStatus.PXCM_STATUS_NO_ERROR && hand != null)
                {
//                    // Querying Hand 2D Position
//                    var massCenterImage = hand.QueryMassCenterImage();
////                    Console.WriteLine(@"Hand position on image: {0} | {1}", massCenterImage.x, massCenterImage.y);
//
//                    // Querying Hand 3D Position
//                    var massCenterWorld = hand.QueryMassCenterWorld();
////                    Console.WriteLine(@"Hand position on world: {0} | {1} | {2}", massCenterWorld.x, massCenterWorld.y,
//                        massCenterWorld.z);

                    // Querying Hand Joints
                    if (hand.HasTrackedJoints())
                    {
                        //searching for location of center hand
                        var jointType = PXCMHandData.JointType.JOINT_MIDDLE_TIP;
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

                            //set 3D position in hand Data (in mm)
                            var p = jointData.positionWorld;
                            _handTrackingData.Location3D = new PXCMPoint3DF32(p.x * 1000, p.y*1000, p.z*1000);
                        }
                    }
                }
            }
        }

        #region Module vars

        private readonly HandTrackingData _handTrackingData;
        private readonly HandTrackingSettings _handTrackingSettings;

        #endregion

        #region RealSense vars

        private PXCMHandModule _handModule;
        private PXCMHandConfiguration _handConfiguration;
        private PXCMHandData _handData;

        #endregion


    }
}