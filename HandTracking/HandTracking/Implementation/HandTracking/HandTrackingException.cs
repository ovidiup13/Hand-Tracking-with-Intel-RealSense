using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandTracking.Interfaces.Module;

namespace HandTracking.Implementation.HandTracking
{
    class HandTrackingNotInitializedException : ModuleException
    {
        public HandTrackingNotInitializedException() : base() { }
        public HandTrackingNotInitializedException(string message) : base(message) { }
        public HandTrackingNotInitializedException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected HandTrackingNotInitializedException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
        { }
    }
}
