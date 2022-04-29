using System.Collections.Generic;

namespace OrigoApiGateway.Models
{
    public class NewLifeCycleSetting
    {
        public bool BuyoutAllowed { get; set; }
        public IList<NewCategoryLifeCycleSetting> CategoryLifeCycleSetting { get; set; }

    }
}
