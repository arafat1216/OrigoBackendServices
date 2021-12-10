using Boilerplate.EntityFramework.Generics.Repositories;

namespace Boilerplate.EntityFramework.UnitTest
{
    internal class ItemRepository : TemporalReadWriteRepository<TestingContext, Item>
    {
        public ItemRepository(ref TestingContext context) : base(ref context)
        {
        }
    }
}
