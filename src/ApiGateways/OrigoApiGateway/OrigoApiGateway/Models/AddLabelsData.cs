using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models
{
    /// <summary>
    /// Request object.
    /// </summary>
    public class AddLabelsData
    {
        public IList<NewLabel> NewLabels { get;  set; }
        public Guid CallerId { get; set; }
    }
}
