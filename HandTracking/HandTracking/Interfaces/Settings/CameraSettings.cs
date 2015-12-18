using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandTracking.Interfaces.Settings
{
    class CameraSettings : ISettings
    {

        public int Width { get; set; }
        public int Height { get; set; }
        public int FramesPerSecond { get; set; }

        public CameraSettings(int width, int height, int fps)
        {
            Width = width;
            Height = height;
            FramesPerSecond = fps;
        }

        PXCMCalibration calibrateCamera()
        {
            return null;
        }

    }
}
