using System;
using System.Threading;
using Aruco.Net;
using HandTracking.Interfaces.Module;
using HandTracking.Interfaces.Settings;
using OpenCV.Net;

namespace HandTracking.Implementation.MarkerTracking
{
    //TODO: replace all errors with exceptions
    internal class MarkerTrackingImpl : Tracking
    {
        /// <summary>
        ///     Constructor that instantiates the Marker Tracking class with default settings.
        /// </summary>
        protected internal MarkerTrackingImpl()
        {
            _markerData = new MarkerData();
            _markerTrackingSettings = new MarkerTrackingSettings();
        }

        /// <summary>
        ///     Constructor that instantiates the Marker Tracking class with custom settings.
        /// </summary>
        /// <param name="settings"></param>
        protected internal MarkerTrackingImpl(ISettings settings)
        {
            _markerData = new MarkerData();
            _markerTrackingSettings = (MarkerTrackingSettings) settings;
        }

        public override void InitializeCameraModules()
        {
            //init session
            Session = PXCMSession.CreateInstance();

            if (Session == null)
                throw new MarkerTrackingException(@"Failed to create Session");

            //init sense manager
            SenseManager = Session.CreateSenseManager();

            if (SenseManager == null)
                throw new MarkerTrackingException(@"Failed to create Sense Manager");

            //initialize image and depth streams
            var imageStatus = SenseManager.EnableStream(PXCMCapture.StreamType.STREAM_TYPE_COLOR,
                _markerTrackingSettings.Width,
                _markerTrackingSettings.Height, _markerTrackingSettings.FramesPerSecond);

            if (imageStatus != pxcmStatus.PXCM_STATUS_NO_ERROR)
                throw new MarkerTrackingException(@"Failed to enable Image Stream.");

            //initialize image and depth streams
            var depthStatus = SenseManager.EnableStream(PXCMCapture.StreamType.STREAM_TYPE_DEPTH,
                _markerTrackingSettings.Width,
                _markerTrackingSettings.Height, _markerTrackingSettings.FramesPerSecond);

            if (depthStatus != pxcmStatus.PXCM_STATUS_NO_ERROR)
                throw new MarkerTrackingException(@"Failed to enable Depth Stream.");

            var initStatus = SenseManager.Init();

            //initialize sense manager
            if (initStatus != pxcmStatus.PXCM_STATUS_NO_ERROR)
                throw new MarkerTrackingException(@"Failed to initialize Sense Manager.");

            _device = SenseManager.captureManager.QueryDevice();
            if (_device == null)
                throw new MarkerTrackingException(@"Failed to Query device.");

            //
            _projection = _device.CreateProjection();
            if (_projection == null)
                throw new MarkerTrackingException(@"Failed to create projection.");

            _markerDetector = new MarkerDetector();
            if (_markerDetector == null)
                throw new MarkerTrackingException(@"Failed to initialize the Aruco Marker Detector.");

            IsInitialized = true;
        }

        public override void StartProcessing()
        {
            if (!IsInitialized)
                throw new MarkerTrackingException(@"Marker Tracking RealSense Modules have not been initialized.");
            if (IsProcessing)
                throw new MarkerTrackingException(@"Marker Tracking process is already running.");

            ProcessingThread = new Thread(TrackingThread);
            IsProcessing = true;
            ProcessingThread.Start();
        }

        /// <summary>
        ///     Method that is run in a separate thread. It runs in an infinite loop and queries
        ///     the Sense Manager for a new frame. The frame is then processed in ProcessFrame method.
        /// </summary>
        protected override void TrackingThread()
        {
            Console.WriteLine(@"Marker Tracking started.");

            ProcessingFlag = true;
            var frameCount = -1;
            while (ProcessingFlag)
            {
                // Acquiring a frame
                if (SenseManager.AcquireFrame(true) < pxcmStatus.PXCM_STATUS_NO_ERROR)
                {
                    break;
                }

                // retrieve the sample and process it
                // counts how many times per second the frame is processed
                if (frameCount++ % 10 == 0)
                {
                    var sample = SenseManager.QuerySample();
                    ProcessFrame(sample);
                }

                //release frame
                SenseManager.ReleaseFrame();
            }

            _device.Dispose();
            _projection.Dispose();
            SenseManager.Close();
            SenseManager.Dispose();
            Session.Dispose();

            Console.WriteLine(@"Marker Tracking terminated.");
        }

        /// <summary>
        ///     Method that processes frames queried by the sense manager. It retrieves
        ///     color and depth streams from the frame, converts the color frame to OpenCV format.
        /// </summary>
        private void ProcessFrame(PXCMCapture.Sample sampleFrame)
        {
            //get depth and color images
            var color = sampleFrame.color;
            var depth = sampleFrame.depth;

            //convert color to OpenCV format
            var colorOcv = PxcmImageToOpenCv(color, PXCMImage.PixelFormat.PIXEL_FORMAT_RGB24);

            //detect markers using Aruco
            var detectedMarkers = _markerDetector.Detect(colorOcv, new CameraParameters());

            PXCMPointF32[] colorPoints = new PXCMPointF32[detectedMarkers.Count];
            PXCMPointF32[] depthPoints = new PXCMPointF32[detectedMarkers.Count];

            //get centroid of markers
            for (var markerIndex = 0; markerIndex < detectedMarkers.Count; markerIndex++)
            {
                var marker = detectedMarkers[markerIndex];
//                Console.WriteLine(@"Marker detected with id " + marker.Id);
                colorPoints[markerIndex] = new PXCMPointF32(marker.Center.X, marker.Center.Y);
            }

            //we map center marker color points to their associated depth point
            _projection.MapColorToDepth(depth, colorPoints, depthPoints);

            //query vertices (which are depth points coordinates in mm)
            PXCMPoint3DF32[] vertices = new PXCMPoint3DF32[depth.info.width * depth.info.height];
            _projection.QueryVertices(depth, vertices);

            for (var point = 0; point < depthPoints.Length; point++)
            {
                var detectedPoint = depthPoints[point];
                if (detectedPoint.x < 0 || detectedPoint.y < 0)
                {
                    Console.WriteLine(@"Marker " + detectedMarkers[point].Id + " is out of range");

                }
                else
                {
                    var v = vertices[(int) (depthPoints[point].y*depth.info.width + depthPoints[point].x)];
                    Console.WriteLine(@"Marker " + detectedMarkers[point].Id + " has coordinates: X:" + v.x + " Y:" + v.y +
                                      @" Z:" + v.z);
                }
            }


            color.Dispose();
            depth.Dispose();
            colorOcv.Dispose();
        }

        public override void StopProcessing()
        {
            if (!IsInitialized)
                throw new MarkerTrackingException(@"Marker Tracking RealSense Modules have not been initialized.");
            if (!IsProcessing)
                throw new MarkerTrackingException(@"Marker Tracking process is not running.");

            ProcessingFlag = false;
            ProcessingThread = null;
            IsProcessing = false;
        }

        public override void PauseProcessing()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     CV_8U - 8-bit unsigned integers ( 0..255 )
        ///     CV_8S - 8-bit signed integers( -128..127 )
        ///     CV_16U - 16-bit unsigned integers( 0..65535 )
        ///     CV_16S - 16-bit signed integers( -32768..32767 )
        ///     CV_32S - 32-bit signed integers( -2147483648..2147483647 )
        ///     CV_32F - 32-bit floating-point numbers( -FLT_MAX..FLT_MAX, INF, NAN )
        ///     CV_64F - 64-bit floating-point numbers( -DBL_MAX..DBL_MAX, INF, NAN )
        /// </summary>
        /// <returns></returns>
        private Mat PxcmImageToOpenCv(PXCMImage image, PXCMImage.PixelFormat format)
        {
            //get image data
            PXCMImage.ImageData imageData;
            image.AcquireAccess(PXCMImage.Access.ACCESS_READ, format, out imageData);

            //get image info
            var width = image.QueryInfo().width;
            var height = image.QueryInfo().height;

            //switch format from RealSense to OpenCV
            var channels = 1;
            var depthType = Depth.U8;
            if (format == PXCMImage.PixelFormat.PIXEL_FORMAT_Y8)
            {
                depthType = Depth.U8;
                channels = 1;
            }
            else if (format == PXCMImage.PixelFormat.PIXEL_FORMAT_RGB24)
            {
                depthType = Depth.U8;
                channels = 3;
            }
            else if (format == PXCMImage.PixelFormat.PIXEL_FORMAT_DEPTH_F32)
            {
                depthType = Depth.F32;
                channels = 1;
            }

            //convert to OpenCV format and pass first pointer to RealSense image
            var openCvImage = new Mat(new Size(width, height), depthType, channels, imageData.planes[0]);

            //release access to image data
            image.ReleaseAccess(imageData);

            return openCvImage;
        }

        /// <summary>
        ///     Method that returns the marker tracking data object associated with tracking.
        /// </summary>
        /// <returns></returns>
        public override IData GetData()
        {
            return _markerData;
        }

        #region tracking vars

        private readonly MarkerTrackingSettings _markerTrackingSettings;
        private readonly MarkerData _markerData;

        //aruco detector
        private MarkerDetector _markerDetector;

        #endregion

        #region RealSense vars

        private PXCMCapture.Device _device;
        private PXCMProjection _projection;

        #endregion
    }
}