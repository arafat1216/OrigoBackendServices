using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.Exceptions
{
    [Serializable]
    public class RootDepartmentAlreadyExistException : Exception
    {
        public RootDepartmentAlreadyExistException() { }

        protected RootDepartmentAlreadyExistException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
