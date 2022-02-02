using System;
using System.ComponentModel.DataAnnotations;

namespace CustomerServices.Models
{
    /// <summary>
    /// Request object
    /// </summary>
    public class CustomerUserCount
    {
        [Required]
        public Guid OrganizationId { get; set; }
        [Required]
        public int Count { get; set; }
    }
}
