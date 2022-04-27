using System;

namespace Asset.API.ViewModels
{
    public class NewLifeCycleSetting
    {
        public bool BuyoutAllowed { get; set; }
        public Guid CallerId { get; set; }
    }
}
