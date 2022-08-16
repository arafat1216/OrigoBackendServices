using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetServices.Exceptions
{
    public class EmailException : AssetException
    {
        public EmailException(string message, Guid traceId, Exception? innerException = null) : base(message, traceId, OrigoErrorCodes.EmailError, innerException)
        {
        }
    }
}
