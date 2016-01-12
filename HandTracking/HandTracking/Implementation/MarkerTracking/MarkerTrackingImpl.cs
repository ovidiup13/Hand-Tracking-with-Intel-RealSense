using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandTracking.Interfaces.Module;
using HandTracking.Interfaces.Settings;

namespace HandTracking.Implementation.MarkerTracking
{
    class MarkerTrackingImpl : Tracking
    {

        protected internal MarkerTrackingImpl()
        {

        }

        protected internal MarkerTrackingImpl(ISettings settings)
        {
            
        }

        public override bool InitializeCameraModules()
        {
            throw new NotImplementedException();
        }

        public override void StartProcessing()
        {
            throw new NotImplementedException();
        }

        public override void StopProcessing()
        {
            throw new NotImplementedException();
        }

        public override void PauseProcessing()
        {
            throw new NotImplementedException();
        }

        public override IData GetData()
        {
            throw new NotImplementedException();
        }
    }
}
