using System;
using System.ComponentModel.DataAnnotations;

namespace Asset.API.ViewModels
{
    public class NewReturnLocation
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string ReturnDescription { get; set; } = string.Empty;
        [Required]
        public Guid LocationId { get; set; }
        public Guid CallerId { get; set; }
    }
}
