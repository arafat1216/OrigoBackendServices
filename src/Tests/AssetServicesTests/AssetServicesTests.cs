using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using AssetServices.Models;
using Xunit;

namespace AssetServicesTests
{
    public class AssetServicesTests : AssetBaseTest
    {

        public AssetServicesTests()
            : base(
                new DbContextOptionsBuilder<AssetsContext>()
                    .UseSqlite("Data Source=sqliteunittests.db")
                    .Options
            )
        {

        }

        [Fact]
        [Trait("Category","UnitTest")]
        public void GetAssetsForUser_ForUserOne_CheckCount()
        {
            // Arrange
            using var context = new AssetsContext(ContextOptions);
            var assetService = new AssetServices.AssetServices(Mock.Of<ILogger<AssetServices.AssetServices>>(), context);

            // Act
            var assetsFromUser = assetService.GetAssetsForUser(ASSETHOLDER_ONE_ID);

            // Assert
            Assert.Equal(2, assetsFromUser.Count);
        }
    }
}
