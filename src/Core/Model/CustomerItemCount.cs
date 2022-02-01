using System;
using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    /// <summary>
    /// Request object
    /// </summary>
    public class CustomerItemCount
    {
        [Required]
        public Guid OrganizationId { get; set; }
        [Required]
        public int Count { get; set; }
    }
}
