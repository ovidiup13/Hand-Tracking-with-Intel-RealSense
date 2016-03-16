using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using CameraModule.Annotations;
using CameraModule.Interfaces.Settings;

namespace CameraModule.Interfaces.Module
{
    public abstract class Tracking
    {

        /// <summary>
        ///     Method that sets and gets the IData field for this Tracking instance.
        /// </summary>
        public Data Data
        {
            get { return _data; }
            internal set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                _data = value;
            }
        }

        /// <summary>
        ///     Field that sets and gets the ISettings field for this Tracking instance.
        /// </summary>
        public CameraSettings Settings
        {
            get { return _settings; }
            internal set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                _settings = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Method that initializes the camera modules and sets up the module for processing.
        /// </summary>
        /// <returns>True if no erros occurred, false otherwise.</returns>
        public abstract void InitializeCameraModules();

        /// <summary>
        ///     Method that starts the processing thread within the derived types.
        /// </summary>
        public abstract void StartProcessing();

        /// <summary>
        ///     Method that runs in a separate thread and provides tracking calculations/processing.
        /// </summary>
        protected abstract void TrackingThread();

        /// <summary>
        ///     Method that stops the processing thread within derived types.
        /// </summary>
        public abstract void StopProcessing();

        /// <summary>
        /// 
        /// </summary>
        public abstract void ResumeProcessing();

        /// <summary>
        ///     Method that pauses the processig thread within derived types.
        /// </summary>
        public abstract void PauseProcessing();

        /// <summary>
        ///     Returns the data instance.
        /// </summary>
        /// <returns></returns>
        public abstract Data GetData();

        #region new image events

        public delegate void NewImageEventHandler(object sender, NewImageArgs args);

        /// <summary>
        ///     Event arguments class for passing the image bitmap to the main view.
        /// </summary>
        public class NewImageArgs : EventArgs
        {
            public NewImageArgs(PXCMCapture.StreamType streamType, Bitmap bitmap)
            {
                Bitmap = bitmap;
                StreamType = streamType;
            }

            public Bitmap Bitmap { get; private set; }
            private PXCMCapture.StreamType StreamType { get; set; }
        }

        /// <summary>
        ///     Method that returns the distance between two points in 3D space. The two points must
        ///     be measured in the same units. (e.g. either meters or millimeters)
        /// </summary>
        /// <param name="point1">First point</param>
        /// <param name="point2">Second point</param>
        /// <returns>The distance in the unit of measurement between the two points in cm (assumming points are measured in mm)</returns>
        public static double GetDistance(PXCMPoint3DF32 point1, PXCMPoint3DF32 point2)
        {
            //TODO: there is a gap between centre of hand and centre of marker - aprox 3cm
            return
                Math.Sqrt(Math.Pow(point1.x - point2.x, 2) + Math.Pow(point1.y - point2.y, 2) +
                          Math.Pow(point1.z - point2.z, 2)) - Offset;
        }

        #endregion

        #region private vars

        public const int Offset = 2;
        private Data _data;
        private CameraSettings _settings;

        #endregion

        #region RealSense vars

        protected PXCMSession Session;
        protected PXCMSenseManager SenseManager;

        #endregion

        #region threading vars   

        protected Thread ProcessingThread;
        protected bool ProcessingFlag;

        #endregion
    }
}