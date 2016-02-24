using CameraModule.Interfaces.Module;
using CameraModule.Interfaces.Settings;

namespace HandModule.Implementation.HandTracking
{
    /// <summary>
    /// Hand Tracking Module is responsible for creating an instance of a Hand Tracking functionality.
    /// </summary>
    public class HandTrackingModule : IModule
    {
        private static HandTrackingImpl _tracking;

        /// <summary>
        /// Method that returns an instace of Tracking object, with default settings.
        /// </summary>
        /// <returns>Instance of Tracking object</returns>
        public Tracking GetInstance()
        {
            return _tracking ?? (_tracking = new HandTrackingImpl());
        }

        /// <summary>
        /// Method that returns an instance of Tracking object, with custom settings.
        /// </summary>
        /// <param name="settings">An ISettings object specifying settings for the Tracking feature.</param>
        /// <returns></returns>
        public Tracking GetInstance(ISettings settings)
        {
            return _tracking ?? (_tracking = new HandTrackingImpl(settings));
        }
    }
}
