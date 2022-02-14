using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionManagementServices.Models
{
    public class CustomerSubscriptionProduct : SubscriptionProduct
    {
        public CustomerSubscriptionProduct()
        {
        }

        public CustomerSubscriptionProduct(SubscriptionProduct? globalSubscriptionProduct)
        {
            GlobalSubscriptionProduct = globalSubscriptionProduct;
        }

        public string Name { get; set; }

        public SubscriptionProduct? GlobalSubscriptionProduct { get; set; }
    }
}
