using System;

namespace Customer.API.ViewModels
{
    public class DeleteContent
    {
        /// <summary>
        /// Request object
        /// </summary>
        public Guid CallerId { get; set; }
        public bool hardDelete { get; set; }
    }
}
