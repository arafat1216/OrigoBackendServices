using System.Collections.Generic;

namespace Customer.API.ViewModels
{
    /// <summary>
    /// View model class for delivering multiple exception strings when one or more exceptions occur in the service class.
    /// </summary>
    public class ExceptionMessages
    {
        /// <summary>
        /// A list of any errors that may have occurred
        /// </summary>
        public List<string> Exceptions { get; set; }

    }
}
