using System;
using System.ComponentModel.DataAnnotations;

namespace Asset.API.ViewModels
{
    public class NewDisposeSetting
    {
        [Required]
        [RegularExpression("^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,4}$", ErrorMessage = "Invalid Email address!!")]
        public string PayrollContactEmail { get; init; }
        public Guid CallerId { get; set; }
    }
}
