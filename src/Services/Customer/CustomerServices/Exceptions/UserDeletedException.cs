using System;
using System.Runtime.Serialization;

namespace CustomerServices.Exceptions
{
    [Serializable]
    public class UserDeletedException : Exception
    {
        public UserDeletedException() { }

        protected UserDeletedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
