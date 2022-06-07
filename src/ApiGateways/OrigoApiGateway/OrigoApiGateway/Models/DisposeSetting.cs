using System;

namespace OrigoApiGateway.Models
{
    public class DisposeSetting
    {
        public Guid Id { get; init; }
        public string PayrollContactEmail { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
