using System;

namespace Common.Exceptions
{
    public class InvalidLifecycleTypeException : Exception
    {
        public InvalidLifecycleTypeException(string msg) : base(msg)
        {

        }
    }
}
