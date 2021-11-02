using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.Exceptions
{
    public class ParentNotValidException : Exception
    {
        public ParentNotValidException()
        {
        }

        public ParentNotValidException(string message) : base(message)
        {
        }

        public ParentNotValidException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
