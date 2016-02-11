﻿namespace HandTracking.Interfaces.Settings
{
    internal class CameraSettings : ISettings
    {
        /// <summary>
        ///     Constructor that initializes the camera screen with custom values.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="fps"></param>
        public CameraSettings(int width, int height, int fps)
        {
            InitializeScreen(width, height, fps);
        }

        /// <summary>
        ///     Constructor that initializes the camera screen with default values.
        /// </summary>
        protected CameraSettings()
        {
            InitializeScreen(_defaultWidth, _defaultHeight, _defaultFps);
        }

        public int Width { get; protected set; } = _defaultWidth;

        public int Height { get; protected set; } = _defaultHeight;

        public int FramesPerSecond { get; set; } = _defaultFps;

        /// <summary>
        ///     Method that sets the widht, height and fps fields.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="fps"></param>
        private void InitializeScreen(int width, int height, int fps)
        {
            Width = width;
            Height = height;
            FramesPerSecond = fps;
        }

        #region default values

        private static readonly int _defaultWidth = 640;
        private static readonly int _defaultHeight = 480;
        private static readonly int _defaultFps = 60;

        #endregion
    }
}