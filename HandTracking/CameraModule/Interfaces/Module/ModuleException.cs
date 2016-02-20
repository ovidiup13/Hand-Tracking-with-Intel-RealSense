using System;

namespace CameraModule.Interfaces.Module
{
    public class ModuleException : Exception
    {
        public ModuleException() : base() { }
        public ModuleException(string message) : base(message) { }
        public ModuleException(string message, System.Exception inner) : base(message, inner) { }

        // A constructor is needed for serialization when an
        // exception propagates from a remoting server to the client. 
        protected ModuleException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
        { }
    }
}
