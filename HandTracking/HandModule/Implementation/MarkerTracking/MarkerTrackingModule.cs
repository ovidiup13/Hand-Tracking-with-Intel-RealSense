﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary1.Interfaces.Module;
using ClassLibrary1.Interfaces.Settings;
using HandTracking.Interfaces.Settings;

namespace HandTracking.Implementation.MarkerTracking
{
    class MarkerTrackingModule : IModule
    {
        /// <summary>
      /// Method that returns an instace of Tracking object, with default settings.
      /// </summary>
      /// <returns>Instance of Tracking object</returns>
        public override Tracking GetInstance()
        {
            return Tracking ?? (Tracking = new MarkerTrackingImpl());
        }

        /// <summary>
        /// Method that returns an instance of Tracking object, with custom settings.
        /// </summary>
        /// <param name="settings">An ISettings object specifying settings for the Tracking feature.</param>
        /// <returns></returns>
        public override Tracking GetInstance(ISettings settings)
        {
            return Tracking ?? (Tracking = new MarkerTrackingImpl(settings));
        }
    }
}
