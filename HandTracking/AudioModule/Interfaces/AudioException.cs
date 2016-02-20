using System;

namespace AudioModule.Interfaces
{
    class AudioException : Exception
    {
        public AudioException() : base() { }
        public AudioException(string message) : base(message) { }
        public AudioException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected AudioException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
        { }
    }
}
