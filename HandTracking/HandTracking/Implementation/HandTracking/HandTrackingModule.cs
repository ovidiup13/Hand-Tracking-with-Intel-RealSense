using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandTracking.Interfaces.Module;
using HandTracking.Interfaces.Settings;

namespace HandTracking.Implementation.HandTracking
{
    /// <summary>
    /// Hand Tracking Module is responsible for creating an instance of a Hand Tracking functionality.
    /// </summary>
    class HandTrackingModule : IModule
    {
        /// <summary>
        /// Method that returns an instace of Tracking object, with default settings.
        /// </summary>
        /// <returns>Instance of Tracking object</returns>
        public override ITracking GetInstace()
        {
            return Tracking ?? (Tracking = new TrackingImpl());
        }

        /// <summary>
        /// Method that returns an instance of Tracking object, with custom settings.
        /// </summary>
        /// <param name="settings">An ISettings object specifying settings for the Tracking feature.</param>
        /// <returns></returns>
        public override ITracking GetInstance(ISettings settings)
        {
            return Tracking ?? (Tracking = new TrackingImpl(settings));
        }
    }
}
