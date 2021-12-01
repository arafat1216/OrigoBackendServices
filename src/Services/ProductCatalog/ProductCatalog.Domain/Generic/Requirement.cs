namespace ProductCatalog.Domain.Generic
{
    public class Requirement
    {
        public IEnumerable<int> Excludes { get; set; }

        public IEnumerable<int> RequiresAll { get; set; }

        public IEnumerable<int> RequiresOne { get; set; }


        public Requirement(IEnumerable<int> excludes, IEnumerable<int> requiresAll, IEnumerable<int> requiresOne)
        {
            Excludes = excludes;
            RequiresAll = requiresAll;
            RequiresOne = requiresOne;
        }
    }
}
