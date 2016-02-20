using CameraModule.Interfaces.Module;

namespace HandModule.Implementation.HandTracking
{
    class HandTrackingException : ModuleException
    {
        public HandTrackingException() : base() { }
        public HandTrackingException(string message) : base(message) { }
        public HandTrackingException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected HandTrackingException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
        { }
    }
}
