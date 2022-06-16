using System;
using System.ComponentModel.DataAnnotations;

namespace Asset.API.ViewModels
{
    public class DeleteReturnLocation
    {
        [Required]
        public Guid ReturnLocationId { get; set; }
        public Guid CallerId { get; set; }
    }
}
