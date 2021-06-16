using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Exceptions
{
    public class InvalidLifecycleTypeException : Exception
    {
        public InvalidLifecycleTypeException(string msg) : base(msg)
        {

        }
    }
}
