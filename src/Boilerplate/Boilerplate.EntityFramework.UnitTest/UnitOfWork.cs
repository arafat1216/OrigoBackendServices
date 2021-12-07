using Boilerplate.EntityFramework.Generics.Repositories;
using Boilerplate.EntityFramework.Generics.UnitOfWork;

namespace Boilerplate.EntityFramework.UnitTest
{
    internal class UnitOfWork : UnitOfWorkRoot<TestingContext>
    {
        private ItemRepository Item { get; set; }
        private TagRepository Tag { get; set; }


        public UnitOfWork(ref TestingContext context) : base(ref context)
        {
            Item = new ItemRepository(ref context);
            Tag = new TagRepository(ref context);
        }
    }
}
