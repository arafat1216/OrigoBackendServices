using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;

namespace Boilerplate.EntityFramework.UnitTest
{
    internal class TestingContextFactory : IDisposable
    {
        private readonly DbConnection _connection;


        public TestingContextFactory()
        {
            //var = new DbContextOptions<TestingContext>();
        }


        private TestingContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TestingContext>();
            optionsBuilder.UseSqlite(CreateInMemoryDatabase());

            return new TestingContext(optionsBuilder.Options);
        }


        /// <summary>
        ///     Sets up a new in-memory Sqlite DB.
        /// </summary>
        /// <returns></returns>
        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            return connection;
        }

        public void Dispose() => _connection.Dispose();


        private void Seed()
        {
            /*
            using (var context = new TestingContext(ContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var one = new Item("ItemOne");
                one.AddTag("Tag11");
                one.AddTag("Tag12");
                one.AddTag("Tag13");

                var two = new Item("ItemTwo");

                var three = new Item("ItemThree");
                three.AddTag("Tag31");
                three.AddTag("Tag31");
                three.AddTag("Tag31");
                three.AddTag("Tag32");
                three.AddTag("Tag32");

                context.AddRange(one, two, three);

                context.SaveChanges();
            }
            */
        }
    }
}