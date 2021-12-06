using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Domain.Generic
{
    public class Requirement
    {
        [Required]
        public IEnumerable<int> Excludes { get; set; }

        [Required]
        public IEnumerable<int> RequiresAll { get; set; }

        [Required]
        public IEnumerable<int> RequiresOne { get; set; }


        public Requirement(IEnumerable<int> excludes, IEnumerable<int> requiresAll, IEnumerable<int> requiresOne)
        {
            Excludes = excludes;
            RequiresAll = requiresAll;
            RequiresOne = requiresOne;
        }
    }
}
