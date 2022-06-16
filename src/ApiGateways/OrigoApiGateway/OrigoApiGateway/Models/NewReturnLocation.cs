using System;
using System.ComponentModel.DataAnnotations;

namespace OrigoApiGateway.Models
{
    public class NewReturnLocation
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string ReturnDescription { get; set; } = string.Empty;
        [Required]
        public Guid LocationId { get; set; }
    }
}
