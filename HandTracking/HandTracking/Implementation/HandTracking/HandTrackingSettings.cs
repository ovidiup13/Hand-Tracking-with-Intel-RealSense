using HandTracking.Interfaces.Settings;

namespace HandTracking.Implementation.HandTracking
{
    class HandTrackingSettings: CameraSettings
    {

        public HandTrackingSettings(int width, int height, int fps) : base(width, height, fps)
        {
        }


    }
}
