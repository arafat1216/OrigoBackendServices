using Boilerplate.EntityFramework.Generics.Repositories;

namespace Boilerplate.EntityFramework.UnitTest
{
    internal class TagRepository : TemporalReadWriteRepository<TestingContext, Tag>
    {
        public TagRepository(ref TestingContext context) : base(ref context)
        {
        }
    }
}
