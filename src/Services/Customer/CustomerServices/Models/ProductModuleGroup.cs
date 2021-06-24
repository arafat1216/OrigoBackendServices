using Common.Seedwork;
using System.Collections.Generic;

namespace CustomerServices.Models
{
    public class ProductModuleGroup : Entity
    {
        public string Name { get; set; }

        public ICollection<Customer> Customers { get; set; }
    }
}