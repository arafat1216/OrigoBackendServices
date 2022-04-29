using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models
{
    public record LifeCycleSetting
    {
        public Guid Id { get; init; }
        public Guid CustomerId { get; init; }
        public bool BuyoutAllowed { get; set; }
        public DateTime CreatedDate { get; set; }
        public IList<CategoryLifeCycleSetting> CategoryLifeCycleSettings { get; set; }
    }
}
