using System;
using System.Collections.Generic;

namespace Asset.API.ViewModels
{
    public class UpdateCustomerLabelsData
    {
        public IList<Label> Labels { get; set; }
        public Guid CallerId { get; set; }
    }
}
