using System;

namespace OrigoApiGateway.Exceptions
{
    public class InvalidLifecycleTypeException : Exception
    {
        public InvalidLifecycleTypeException(string msg) : base(msg)
        {

        }
    }
}
