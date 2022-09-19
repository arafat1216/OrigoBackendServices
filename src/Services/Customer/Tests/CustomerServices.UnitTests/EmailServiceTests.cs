using Common.Configuration;
using Common.Utilities;
using CustomerServices.Email;
using Moq;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using Xunit;
using System;
using System.Net.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using System.IO;
using CustomerServices.Email.Models;
using System.Collections.Generic;
using Moq.Protected;
using System.Threading;
using System.Net;

namespace CustomerServices.UnitTests
{
    public class EmailServiceTests
    {
        private EmailConfiguration _config;


        public EmailServiceTests()
        {
            _config = new EmailConfiguration()
            {
                OrigoBaseUrl = "https://origov2dev.mytos.no/en",
                LoginPath = "/login",
                MyPagePath = "/my-page/dashboard",
                UserDetailViewPath = "/my-business/{{CustomerId}}/users/{{UserId}}/view"
            };
        }

        [Fact]
        public void GetEmailTemplate()
        {
            var resourceManger = new ResourceManager("CustomerServices.Resources.Customer", Assembly.GetAssembly(typeof(EmailService))!);
            var OrigoInvitationEnglishTemplate = resourceManger.GetString("OrigoInvitation", CultureInfo.CreateSpecificCulture("EN"));
            Assert.NotNull(OrigoInvitationEnglishTemplate);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task OffboardingOverdueEmailToManagersAsync_VerifyThatConfigIsCalled()
        {
            //Arrange
            var mockFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object) { BaseAddress = new Uri("http://localhost") };
            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var mockIOption = new Mock<IOptions<EmailConfiguration>>();
            mockIOption.Setup(ap => ap.Value).Returns(_config);

            var resourceManger = new ResourceManager("CustomerServices.Resources.Customer", Assembly.GetAssembly(typeof(EmailService))!);

            var flatDictionary = new FlatDictionary();
            var emailservice = new EmailService(mockIOption.Object, flatDictionary, resourceManger, mockFactory.Object);

            var offBoardingOverdue = new OffboardingOverdueMail 
            { 
                CustomerId = Guid.NewGuid(), 
                UserId = Guid.NewGuid(), 
                LastWorkingDays = DateTime.UtcNow.ToString(), 
                Recipient = new List<string> {"d@d.com" },
                UserName = "Donald Duck"
            };

            await emailservice.OffboardingOverdueEmailToManagersAsync(offBoardingOverdue, "en");

            mockIOption.Verify(v => v.Value, Times.Once());
        }
    }
}
