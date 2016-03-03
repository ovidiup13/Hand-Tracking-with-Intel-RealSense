using System;
using System.Threading;
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
            Settings = new HandTrackingSettings();
            _handTrackingData = new HandTrackingData();
        }

        /// <summary>
        ///     Create a new HandTrackingImpl instance with custom settings.
        /// </summary>
        /// <param name="settings"></param>
        protected internal HandTrackingImpl(ISettings settings)
        {
            Settings = (HandTrackingSettings) settings;
            _handTrackingData = new HandTrackingData();
        }

        /// <summary>
        ///     Method that starts the HandTracking thread.
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
        ///     Method that stops the HandTracking thread.
        /// </summary>
        public override void StopProcessing()
        {
            //terminate thread
            ProcessingFlag = false;
            ProcessingThread = null;
            IsProcessing = false;
        }

        //TODO: implement pause
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

            IsInitialized = true;
        }

        /// <summary>
        ///     Method that is run in a separate thread.
        /// </summary>
        protected override void TrackingThread()
        {
            Console.WriteLine(@"Hand Tracking Started.");
            ProcessingFlag = true;
            var frameCount = -1;
            // Looping to query the hands information
            while (ProcessingFlag)
            {
                // Acquiring a frame
                if (SenseManager.AcquireFrame(true) < pxcmStatus.PXCM_STATUS_NO_ERROR)
                {
                    break;
                }

                //acquire color stream and pass it to the delegate
                //TODO: check if it works or not
                var sample = SenseManager.QuerySample();
                AccessImage(sample.color);

                if (frameCount++%10 == 0)
                {
                    // Updating the hand data
                    _handData?.Update();

                    // Processing Hands
                    ProcessHands(_handData);
                }

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

        private void AccessImage(PXCMImage image)
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
                _handTrackingData.HandDetected = false;
                Console.WriteLine(@"Hand not detected!");
                return;
            }

            //detected at least one hand
            _handTrackingData.HandDetected = true;
            Console.WriteLine(@"Hand detected! Number of hands: " + numberOfHands);

            // Querying the information about detected hands
            for (var i = 0; i < numberOfHands; i++)
            {
                // Querying hand id
                int handId;

                //closest hand gets smaller id
                var queryHandIdStatus = handData.QueryHandId(Settings.AccessOrderType, i,
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
//                Console.WriteLine(@"Hand data status: " + queryHandStatus);

                //TODO: refactor hand position so that it updates accordingly with user settings
                if (queryHandStatus == pxcmStatus.PXCM_STATUS_NO_ERROR && hand != null)
                {
//                    // Querying Hand 2D Position
                    /*PXCMPointF32 massCenterImage = hand.QueryMassCenterImage();
//                    Console.WriteLine(@"Hand position on image: {0} | {1}", massCenterImage.x, massCenterImage.y);


                    // Querying Hand 3D Position
                    PXCMHandData.ExtremityData location3D;
                    var massCenterWorld = hand.QueryExtremityPoint(PXCMHandData.ExtremityType.EXTREMITY_CENTER, out location3D);
//                                        Console.WriteLine(@"Hand position on world: {0} | {1} | {2}", location3D.pointWorld.x, location3D.pointWorld.y,
//                                            location3D.pointWorld.z);*/

                    /* var location = hand.QueryMassCenterWorld();

                    _handTrackingData.Location2D = location3D.pointImage;
                    //                    _handTrackingData.Location3D = new PXCMPoint3DF32(location3D.pointWorld.x * 1000, location3D.pointWorld.y * 1000, location3D.pointWorld.z * 1000);
                    _handTrackingData.Location3D = new PXCMPoint3DF32(location.x * 100, location.y * 1000, location.z * 1000);*/

                    // Querying Hand Joints
                    if (hand.HasTrackedJoints())
                    {
                        //searching for location of center hand
                        var jointType = Settings.JointType;
                        PXCMHandData.JointData jointData;
                        var queryStatus = hand.QueryTrackedJoint(jointType, out jointData);

                        if (queryStatus == pxcmStatus.PXCM_STATUS_NO_ERROR && jointData != null)
                        {
                            //                            // Printing the 2D position (image)
                            //                            Console.WriteLine(@"	2D Position: {0} | {1}", jointData.positionImage.x,
                            //                                jointData.positionImage.y);

                            //set 2D position in hand Data
                            _handTrackingData.Location2D = jointData.positionImage;

                            // Printing the 3D position (depth)
                            Console.WriteLine(@"	3D Position: {0} | {1} | {2}", jointData.positionWorld.x,
                                jointData.positionWorld.y, jointData.positionWorld.z);

                            //set 3D position in hand Data (in mm)
                            var p = jointData.positionWorld;
                            _handTrackingData.Location3D = new PXCMPoint3DF32(p.x*1000, p.y*1000, p.z*1000);
                        }
                    }
                }
            }
        }

        #region Module vars

        public event NewImageEventHandler NewImageAvailable;


        private readonly HandTrackingData _handTrackingData;
        public new HandTrackingSettings Settings { get; }

        #endregion

        #region RealSense vars

        private PXCMHandModule _handModule;
        private PXCMHandConfiguration _handConfiguration;
        private PXCMHandData _handData;

        #endregion
    }
}