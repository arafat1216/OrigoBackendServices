using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models
{
    /// <summary>
    /// Request object.
    /// </summary>
    public class DeleteCustomerLabelsData
    {
        public IList<Guid> LabelGuids { get; set; }
        public Guid CallerId { get; set; }
    }
}
