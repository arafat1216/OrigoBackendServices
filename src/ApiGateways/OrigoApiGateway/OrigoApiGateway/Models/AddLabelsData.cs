using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Models
{
    public class AddLabelsData
    {
        public IList<NewLabel> NewLabels { get;  set; }
        public Guid CallerId { get; set; }
    }
}
