using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CustomerServices.UnitTests
{
    public class WebshopServiceTests
    {
        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task CheckAndProvisionWebShopUser_NullUser()
        {
            ServiceModels.OktaUserDTO oktaUserDTO = null!;
            var oktaMock = new Mock<IOktaServices>();
            var webshopConFigMock = new Mock<IOptions<WebshopConfiguration>>();
            oktaMock.Setup(m => m.GetOktaUserProfileByLoginEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(oktaUserDTO);

            var webshopService = new WebshopService(oktaMock.Object, webshopConFigMock.Object);
            
            var exception = await Record.ExceptionAsync(() =>
                webshopService.CheckAndProvisionWebShopUserAsync(It.IsAny<string>())
            );

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("General request towards web shop failed.", exception.Message);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task CheckAndProvisionWebShopUser_EmptyOrgNumber()
        {
            ServiceModels.OktaUserDTO oktaUserDTO = new ServiceModels.OktaUserDTO
            {
                Profile = new ServiceModels.OktaUserProfile
                {
                    OrganizationNumber = ""
                }
            };

            var oktaMock = new Mock<IOktaServices>();
            var webshopConFigMock = new Mock<IOptions<WebshopConfiguration>>();
            oktaMock.Setup(m => m.GetOktaUserProfileByLoginEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(oktaUserDTO);

            var webshopService = new WebshopService(oktaMock.Object, webshopConFigMock.Object);

            var exception = await Record.ExceptionAsync(() =>
                webshopService.CheckAndProvisionWebShopUserAsync(It.IsAny<string>())
            );

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("User missing organization number.", exception.Message);
        }
    }
}
