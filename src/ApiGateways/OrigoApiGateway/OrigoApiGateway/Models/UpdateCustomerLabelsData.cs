using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models
{
    /// <summary>
    /// Request object
    /// </summary>
    public class UpdateCustomerLabelsData
    {
        public IList<Label> Labels { get; set; }
        public Guid CallerId { get; set; }
    }
}
