using System;
using System.Collections.Generic;

namespace Asset.API.ViewModels
{
    public class DeleteCustomerLabelsData
    {
        public IList<Guid> LabelGuids { get; set; }
        public Guid CallerId { get; set; }
    }
}
