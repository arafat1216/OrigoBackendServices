using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetServices.Exceptions
{
    public class InvalidLifecycleTypeException : Exception
    {
        public InvalidLifecycleTypeException(string msg) : base(msg)
        {

        }
    }
}
