using System;
using System.ComponentModel.DataAnnotations;

namespace OrigoApiGateway.Models
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
        public int NotOnboarded { get; set; }

    }
}
