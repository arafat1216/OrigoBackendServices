using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using OrigoAssetServices.Models;
using OrigoAssetServices.Services;
using Xunit;

namespace OrigoAssetServicesTests
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
                var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), context);

                // Act
                var assetsFromUser = assetService.GetAssetsForUser(ASSETHOLDER_ONE_ID);

                // Assert
                Assert.Equal(2, assetsFromUser.Count);
            }
        }
    }
}
