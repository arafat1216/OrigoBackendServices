using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.Exceptions
{
    /// <summary>
    /// When an entity has IsDeleted == true, throw this exception when suitable
    /// </summary>
    public class EntityIsDeletedException : Exception
    {
    }
}
