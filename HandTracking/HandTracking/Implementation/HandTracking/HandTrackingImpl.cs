using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandTracking.Interfaces.Module;
using HandTracking.Interfaces.Settings;

namespace HandTracking.Implementation.HandTracking
{
    class HandTrackingImpl : ITracking
    {

        protected internal HandTrackingImpl()
        {
            
        }

        protected internal HandTrackingImpl(ISettings settings)
        {
            
        }

        public void StartProcessing()
        {
            throw new NotImplementedException();
        }

        public void StopProcessing()
        {
            throw new NotImplementedException();
        }

        public void PauseProcessing()
        {
            throw new NotImplementedException();
        }

        public IData GetData()
        {
            throw new NotImplementedException();
        }
    }
}
