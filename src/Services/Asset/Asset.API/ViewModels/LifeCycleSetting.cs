using System;
using System.Collections.Generic;

namespace Asset.API.ViewModels
{
    public class LifeCycleSetting
    {
        public Guid Id { get; init; }
        public Guid CustomerId { get; init; }
        public bool BuyoutAllowed { get; set; }
        public DateTime CreatedDate { get; set; }
        public IList<CategoryLifeCycleSetting> CategoryLifeCycleSettings { get; init; }
    }
}
