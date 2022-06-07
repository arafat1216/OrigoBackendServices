using System;
using System.ComponentModel.DataAnnotations;

namespace Asset.API.ViewModels
{
    public class DisposeSetting
    {
        public Guid Id { get; init; }
        public Guid CustomerId { get; init; }
        public string PayrollContactEmail { get; init; }
        public DateTime CreatedDate { get; set; }
    }
}
