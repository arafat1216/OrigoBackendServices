using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using AssetServices.Models;
using AssetServices.Services;
using Xunit;

namespace AssetServicesTests
{
    public class AssetServicesTests : AssetBaseTest
    {

        public AssetServicesTests()
            : base(
                new DbContextOptionsBuilder<AssetsContext>()
                    .UseSqlite()
                    .Options
            )
        {

        }

        [Fact]
        [Trait("Category","UnitTest")]
        public void GetAssetsForUser_ForUserOne_CheckCount()
        {
            // Arrange
            using (var context = new AssetsContext(ContextOptions))
            {
                var assetService = new AssetServices.Services.AssetServices(Mock.Of<ILogger<AssetServices.Services.AssetServices>>(), context);

                // Act
                var assetsFromUser = assetService.GetAssetsForUser(ASSETHOLDER_ONE_ID);

                // Assert
                Assert.Equal(2, assetsFromUser.Count);
            }
        }
    }
}
