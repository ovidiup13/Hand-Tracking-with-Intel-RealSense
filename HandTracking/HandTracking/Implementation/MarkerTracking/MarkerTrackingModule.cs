using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandTracking.Interfaces.Module;
using HandTracking.Interfaces.Settings;

namespace HandTracking.Implementation.MarkerTracking
{
    class MarkerTrackingModule : IModule
    {
        public override ITracking GetInstance(ISettings settings)
        {
            throw new NotImplementedException();
        }

        public override ITracking GetInstance()
        {
            throw new NotImplementedException();
        }
    }
}
