using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandTracking.Interfaces.Settings
{
    class HandTrackingSettings: CameraSettings
    {


        public HandTrackingSettings(int width, int height, int fps) : base(width, height, fps)
        {
        }


    }
}
