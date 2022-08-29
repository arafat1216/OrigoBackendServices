using System.Collections.Generic;

namespace CustomerServices.ServiceModels
{
    /// <summary>
    /// Service model class for delivering multiple exception strings when one or more exceptions occur in service class.
    /// </summary>
    public class ExceptionMessagesDTO
    {
        /// <summary>
        /// A list of any exceptions that may have occurred.
        /// </summary>
        public List<string> Exceptions { get; set; } = new List<string>();
    }
}
