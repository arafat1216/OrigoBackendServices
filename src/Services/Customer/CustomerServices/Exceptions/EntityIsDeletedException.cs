using System;

namespace CustomerServices.Exceptions
{
    /// <summary>
    /// When an entity has IsDeleted == true, throw this exception when suitable
    /// </summary>
    public class EntityIsDeletedException : Exception
    {
    }
}
