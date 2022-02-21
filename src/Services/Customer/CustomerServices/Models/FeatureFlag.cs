using System;
using Common.Seedwork;

namespace CustomerServices.Models
{
    public class FeatureFlag : Entity
    {
        public string Name { get; set; }
        public Guid? CustomerId { get; set; }
    }
}
