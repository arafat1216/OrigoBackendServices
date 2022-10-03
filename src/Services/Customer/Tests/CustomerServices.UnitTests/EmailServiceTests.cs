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
using System.IO;
using CustomerServices.Email.Models;
using System.Collections.Generic;
using Moq.Protected;
using System.Threading;
using System.Net;
using CustomerServices.Email.Configuration;

namespace CustomerServices.UnitTests
{
    public class EmailServiceTests
    {
        private EmailConfigurationCustomer _config;


        public EmailServiceTests()
        {
            _config = new EmailConfigurationCustomer()
            {
                OrigoBaseUrl = "https://origov2dev.mytos.no/en",
                LoginPath = "/login",
                MyPagePath = "/my-page/dashboard",
                UserDetailViewPath = "/my-business/{{CustomerId}}/users/{{UserId}}/view"
            };
        }

        [Fact]
        public void GetEmailTemplate_English()
        {
            var resourceManger = new ResourceManager("CustomerServices.Resources.Customer", Assembly.GetAssembly(typeof(EmailService))!);
            var OrigoInvitationEnglishTemplate = resourceManger.GetString("OrigoInvitation", CultureInfo.CreateSpecificCulture("en"));
            Assert.NotNull(OrigoInvitationEnglishTemplate);
            Assert.Equal("### Hello {{FirstName}}!\r\n\r\nOrigo is a self service portal to help you and your business to keep control of your assets.\r\n\r\nIn order for you to start using Origo, please go through our onboarding tour:\r\n\r\n[Start Onboarding]({{OrigoBaseUrl}})\r\n", OrigoInvitationEnglishTemplate);
        }

        [Fact]
        public void GetEmailTemplate_Norwgian_no()
        {
            var resourceManger = new ResourceManager("CustomerServices.Resources.Customer", Assembly.GetAssembly(typeof(EmailService))!);
            var OrigoInvitationNorwegianTemplate = resourceManger.GetString("OrigoInvitation", CultureInfo.CreateSpecificCulture("no"));
            Assert.NotNull(OrigoInvitationNorwegianTemplate);
            Assert.Equal("### Hello {{FirstName}}!\r\n\r\nOrigo er en selvbetjeningsportal for å hjelpe deg og din bedrift med å holde kontroll over dine eiendeler.\r\n\r\nFor at du skal begynne å bruke Origo, vennligst gå gjennom vår onboarding tour:\r\n\r\n[Start Onboarding]({{OrigoBaseUrl}})",OrigoInvitationNorwegianTemplate);
        }
        [Fact]
        public void GetEmailTemplate_Norwgian_UpperCaseLanguageCode()
        {
            var resourceManger = new ResourceManager("CustomerServices.Resources.Customer", Assembly.GetAssembly(typeof(EmailService))!);
            var OrigoInvitationNorwegianTemplate = resourceManger.GetString("OrigoInvitation", CultureInfo.CreateSpecificCulture("NO"));
            Assert.NotNull(OrigoInvitationNorwegianTemplate);
            Assert.Equal("### Hello {{FirstName}}!\r\n\r\nOrigo er en selvbetjeningsportal for å hjelpe deg og din bedrift med å holde kontroll over dine eiendeler.\r\n\r\nFor at du skal begynne å bruke Origo, vennligst gå gjennom vår onboarding tour:\r\n\r\n[Start Onboarding]({{OrigoBaseUrl}})", OrigoInvitationNorwegianTemplate);
        }

        [Fact]
        public void GetEmailTemplate_Norwgian_nb()
        {
            var resourceManger = new ResourceManager("CustomerServices.Resources.Customer", Assembly.GetAssembly(typeof(EmailService))!);
            var OrigoInvitationNorwegianTemplate = resourceManger.GetString("OrigoInvitation", CultureInfo.CreateSpecificCulture("nb"));
            Assert.NotNull(OrigoInvitationNorwegianTemplate);
            Assert.Equal("### Hello {{FirstName}}!\r\n\r\nOrigo er en selvbetjeningsportal for å hjelpe deg og din bedrift med å holde kontroll over dine eiendeler.\r\n\r\nFor at du skal begynne å bruke Origo, vennligst gå gjennom vår onboarding tour:\r\n\r\n[Start Onboarding]({{OrigoBaseUrl}})", OrigoInvitationNorwegianTemplate);
        }
        [Fact]
        public void GetEmailTemplate_Swedish_sv()
        {
            var resourceManger = new ResourceManager("CustomerServices.Resources.Customer", Assembly.GetAssembly(typeof(EmailService))!);
            var OrigoInvitationSwedishTemplate = resourceManger.GetString("OrigoInvitation", CultureInfo.CreateSpecificCulture("sv"));
            Assert.NotNull(OrigoInvitationSwedishTemplate);
            Assert.Equal("### Hej {{FirstName}}!\r\n\r\nOrigo är en självbetjäningsportal som hjälper dig och ditt företag att behålla kontrollen över dina tillgångar.\r\n\r\nFör att du ska börja använda Origo, gå igenom vår onboarding tour:\r\n\r\n[Start Onboarding]({{OrigoBaseUrl}})", OrigoInvitationSwedishTemplate);
        }
        [Fact]
        public void GetEmailTemplate_SE_ReturnsEnglish()
        {
            var resourceManger = new ResourceManager("CustomerServices.Resources.Customer", Assembly.GetAssembly(typeof(EmailService))!);
            var OrigoInvitationEnglishTemplate = resourceManger.GetString("OrigoInvitation", CultureInfo.CreateSpecificCulture("SE"));
            Assert.NotNull(OrigoInvitationEnglishTemplate);
            Assert.Equal("### Hello {{FirstName}}!\r\n\r\nOrigo is a self service portal to help you and your business to keep control of your assets.\r\n\r\nIn order for you to start using Origo, please go through our onboarding tour:\r\n\r\n[Start Onboarding]({{OrigoBaseUrl}})\r\n", OrigoInvitationEnglishTemplate);
        }
        [Fact]
        public void GetEmailTemplate_DefaultToEnglish_WhenLanguageCodeIsNotOneWeHave()
        {
            var resourceManger = new ResourceManager("CustomerServices.Resources.Customer", Assembly.GetAssembly(typeof(EmailService))!);
            var OrigoInvitationEnglishTemplate = resourceManger.GetString("OrigoInvitation", CultureInfo.CreateSpecificCulture("FR"));
            Assert.NotNull(OrigoInvitationEnglishTemplate);
            Assert.Equal("### Hello {{FirstName}}!\r\n\r\nOrigo is a self service portal to help you and your business to keep control of your assets.\r\n\r\nIn order for you to start using Origo, please go through our onboarding tour:\r\n\r\n[Start Onboarding]({{OrigoBaseUrl}})\r\n", OrigoInvitationEnglishTemplate);
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

            var mockIOption = new Mock<IOptions<EmailConfigurationCustomer>>();
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
