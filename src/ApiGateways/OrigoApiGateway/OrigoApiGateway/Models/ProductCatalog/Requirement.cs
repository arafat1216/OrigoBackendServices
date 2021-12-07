using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrigoApiGateway.Models.ProductCatalog
{
    public class Requirement
    {
        [Required]
        public IEnumerable<int> Excludes { get; set; }

        [Required]
        public IEnumerable<int> RequiresAll { get; set; }

        [Required]
        public IEnumerable<int> RequiresOne { get; set; }


        public Requirement()
        {
            Excludes = new List<int>();
            RequiresAll = new List<int>();
            RequiresOne = new List<int>();
        }


        public Requirement(IEnumerable<int> excludes, IEnumerable<int> requiresAll, IEnumerable<int> requiresOne)
        {
            Excludes = excludes;
            RequiresAll = requiresAll;
            RequiresOne = requiresOne;
        }
    }
}
