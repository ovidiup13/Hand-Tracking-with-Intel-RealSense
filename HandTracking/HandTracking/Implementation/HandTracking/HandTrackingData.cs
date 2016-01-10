using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandTracking.Interfaces.Module;

namespace HandTracking.Implementation.HandTracking
{
    class HandTrackingData : IData
    {
        public PXCMPoint3DF32 getHandPosition3D()
        {
            throw new NotImplementedException();
        }
    }
}
