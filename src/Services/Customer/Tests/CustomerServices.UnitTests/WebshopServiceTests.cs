using CustomerServices.Models;
using CustomerServices.ServiceModels;
using Microsoft.Extensions.Options;

namespace CustomerServices.UnitTests
{
    public class WebshopServiceTests
    {
        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task CheckAndProvisionImplementWebShopUser_NullUser()
        {
            ServiceModels.OktaUserDTO oktaUserDTO = null!;
            var oktaMock = new Mock<IOktaServices>();
            var webshopConFigMock = new Mock<IOptions<WebshopConfiguration>>();
            var organizationRepositoryMock = new Mock<IOrganizationRepository>();
            var userServicesMock = new Mock<IUserServices>();

            oktaMock.Setup(m => m.GetOktaUserProfileByLoginEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(oktaUserDTO);

            var webshopService = new WebshopService(oktaMock.Object, organizationRepositoryMock.Object, userServicesMock.Object, webshopConFigMock.Object);

            var exception = await Record.ExceptionAsync(() =>
                webshopService.CheckAndProvisionImplementWebShopUserAsync(It.IsAny<string>())
            );

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("General request towards web shop failed.", exception.Message);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task CheckAndProvisionImplementWebShopUser_EmptyOrgNumber()
        {
            OktaUserDTO oktaUserDTO = new()
            {
                Profile = new OktaUserProfile
                {
                    OrganizationNumber = ""
                }
            };

            var oktaMock = new Mock<IOktaServices>();
            var webshopConFigMock = new Mock<IOptions<WebshopConfiguration>>();
            var organizationRepositoryMock = new Mock<IOrganizationRepository>();
            var userServicesMock = new Mock<IUserServices>();

            oktaMock.Setup(m => m.GetOktaUserProfileByLoginEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(oktaUserDTO);

            var webshopService = new WebshopService(oktaMock.Object, organizationRepositoryMock.Object, userServicesMock.Object, webshopConFigMock.Object);

            var exception = await Record.ExceptionAsync(() =>
                webshopService.CheckAndProvisionImplementWebShopUserAsync(It.IsAny<string>())
            );

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("User missing organization number.", exception.Message);
        }

        [Fact, Trait("Category", "UnitTest")]
        public async Task CheckAndProvisionWebShopUser_UserInfoWithInvalidOrganizationId_ThrowsInvalidOrganizationNumberException()
        {
            // Arrange
            var oktaMock = new Mock<IOktaServices>();
            var webshopConFigMock = new Mock<IOptions<WebshopConfiguration>>();
            var organizationRepositoryMock = new Mock<IOrganizationRepository>();
            var userServicesMock = new Mock<IUserServices>();

            var webshopService = new WebshopService(oktaMock.Object, organizationRepositoryMock.Object, userServicesMock.Object, webshopConFigMock.Object);

            UserInfoDTO userInfo = new()
            {
                UserId = Guid.NewGuid(),
                OrganizationId = Guid.Empty
            };

            userServicesMock.Setup(mock => mock.GetUserInfoFromUserId(It.IsAny<Guid>()))
                .ReturnsAsync(userInfo);

            // Act
            Task provisionUser() => webshopService.CheckAndProvisionWebShopUserAsync(userInfo.UserId);

            // Assert
            await Assert.ThrowsAnyAsync<Exception>(provisionUser);
        }

        [Fact, Trait("Category", "UnitTest")]
        public async Task ProvisionWebShopUserByOktaEmailAndOrgnumber_InvalidUserInfo_ShouldThrow()
        {
            // Arrange
            var oktaMock = new Mock<IOktaServices>();
            var webshopConFigMock = new Mock<IOptions<WebshopConfiguration>>();
            var organizationRepositoryMock = new Mock<IOrganizationRepository>();
            var userServicesMock = new Mock<IUserServices>();
            OktaUserDTO? oktaUserDTO = null;

            oktaMock.Setup(m => m.GetOktaUserProfileByLoginEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(oktaUserDTO);
            var webshopService = new WebshopService(oktaMock.Object, organizationRepositoryMock.Object, userServicesMock.Object, webshopConFigMock.Object);

            // Act
            Task provisionUser() => webshopService.ProvisionWebShopUserByOktaEmailAndOrgnumberAsync("invalid_email", "invalid_orgnumber");

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(provisionUser);
        }

    }
}
