using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.Exceptions
{
    public class RequiredFieldIsEmptyException : Exception
    {
        public RequiredFieldIsEmptyException()
        {
        }

        public RequiredFieldIsEmptyException(string message) : base(message)
        {
        }

        public RequiredFieldIsEmptyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
