using System;
using System.Runtime.Serialization;

namespace HandTracking.Implementation.MarkerTracking
{
    internal class MarkerTrackingException : ModuleException
    {
        public MarkerTrackingException()
        {
        }

        public MarkerTrackingException(string message) : base(message)
        {
        }

        public MarkerTrackingException(string message, Exception inner) : base(message, inner)
        {
        }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected MarkerTrackingException(SerializationInfo info,
            StreamingContext context)
        {
        }
    }
}