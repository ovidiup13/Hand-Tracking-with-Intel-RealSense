using System;
using HandTracking.Interfaces.Settings;

namespace HandTracking.Implementation.HandTracking
{
    internal class HandTrackingSettings : CameraSettings
    {
        #region private vars

        private float _smoothingValue = 1;

        #endregion

        #region Hand settings vars

        /// <summary>
        /// description of stabilizer does
        /// </summary>
        public bool EnableStabilizer { get; set; } = true;

        /// <summary>
        /// description of smoothing value
        /// </summary>
        public float SmoothingValue
        {
            get { return _smoothingValue;  }
            set
            {
                if(value < 0 || value > 1) throw new ArgumentOutOfRangeException(nameof(value));
                _smoothingValue = value;
            }
        }

        /// <summary>
        /// Joint type to tracked. Can be the center of palm, extremities, etc.
        /// </summary>
        public PXCMHandData.JointType JointType { get; set; } = PXCMHandData.JointType.JOINT_CENTER;

        #endregion

        /// <summary>
        /// Creates a new instance of HandTrackingSettings based on its super class.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="fps"></param>
        public HandTrackingSettings(int width, int height, int fps) : base(width, height, fps)
        {
        }

        /// <summary>
        /// Creates a new instance of HandTrackingSettings with default values.
        /// </summary>
        public HandTrackingSettings()
        {
        }
    }
}