using System;
using System.Collections.Generic;

namespace Asset.API.ViewModels
{
    public class AddLabelsData
    {
        public IList<NewLabel> NewLabels { get; set; }
        public Guid CallerId { get; set; }
    }
}
