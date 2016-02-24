using System;
using CameraModule.Interfaces.Settings;

namespace MarkerTracking.Implementation
{
    internal class MarkerTrackingSettings : CameraSettings
    {
        /// <summary>
        ///     Creates a new instance of HandTrackingSettings based on its super class.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="fps"></param>
        public MarkerTrackingSettings(int width, int height, int fps) : base(width, height, fps)
        {
        }

        /// <summary>
        ///     Creates a new instance of HandTrackingSettings with default values.
        /// </summary>
        public MarkerTrackingSettings()
        {
        }

        #region vars

        private bool _isProcessing;


        #endregion


    }
}