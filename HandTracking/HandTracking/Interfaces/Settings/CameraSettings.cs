using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandTracking.Interfaces.Settings
{
    class CameraSettings : ISettings
    {
        private readonly int _defaultWidth = 640;
        private readonly int _defaultHeight = 480;
        private readonly int _defaultFps = 30;

        public int Width { get; set; }
        public int Height { get; set; }
        public int FramesPerSecond { get; set; }

        /// <summary>
        /// Constructor that initializes the camera screen with custom values.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="fps"></param>
        public CameraSettings(int width, int height, int fps)
        {
            InitializeScreen(width, height, fps);
        }

        /// <summary>
        /// Constructor that initializes the camera screen with default values.
        /// </summary>
        public CameraSettings()
        {
            InitializeScreen(_defaultWidth, _defaultHeight, _defaultFps);
        }

        /// <summary>
        /// Method that sets the widht, height and fps fields.
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

    }
}
