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
            //Arrange
            var language = "en";
            var resourceManger = new ResourceManager("CustomerServices.Resources.Customer", Assembly.GetAssembly(typeof(EmailService)));

            //Act
            var InvitationMailTemplate = resourceManger.GetString(InvitationMail.TemplateName, CultureInfo.CreateSpecificCulture(language));
            var OffboardingInitiatedWithBuyoutTemplate = resourceManger.GetString(OffboardingInitiatedWithBuyout.TemplateName, CultureInfo.CreateSpecificCulture(language));
            var OffboardingInitiatedWithoutBuyoutTemplate = resourceManger.GetString(OffboardingInitiatedWithoutBuyout.TemplateName, CultureInfo.CreateSpecificCulture(language));
            var OffboardingOverdueMailTemplate = resourceManger.GetString(OffboardingOverdueMail.TemplateName, CultureInfo.CreateSpecificCulture(language));

            //Assert
            Assert.NotNull(InvitationMailTemplate);
            Assert.NotNull(OffboardingInitiatedWithBuyoutTemplate);
            Assert.NotNull(OffboardingInitiatedWithoutBuyoutTemplate);
            Assert.NotNull(OffboardingOverdueMailTemplate);

            //.Replace(System.Environment.NewLine, string.Empty)) is used to remove \r\n or \n which leads to unit test failing in docker. 
            Assert.Equal("### Hello {{FirstName}}!Origo is a self service portal to help you and your business to keep control of your assets.In order for you to start using Origo, please go through our onboarding tour:[Start Onboarding]({{OrigoBaseUrl}})", InvitationMailTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("### Hello {{FirstName}}!In regards of your offboarding, you have some tasks to complete in Origo before you leave.1. Choose whether you would like to *Buyout* or *Return* your asset to company. If you choose *Buyout*, this need to be done before {{LastBuyoutDay}}, in order to do a salary deduction. After this date, only *Return* will be available. Visit Origo to see your buyout price.Please follow the steps provided in the portal[Start Offboarding]({{MyPageLink}})", OffboardingInitiatedWithBuyoutTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("### Hello {{FirstName}}!In regards of your offboarding, you have some tasks to complete in Origo before you leave.1. Return your assets to companyPlease follow the steps provided in the portal[Start Offboarding]({{MyPageLink}})", OffboardingInitiatedWithoutBuyoutTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("### Hello!A user has been granted the status \"Offboarding Overdue\".**Name**: {{UserName}} **Last WorkingDay**: {{LastWorkingDays}} The user has now lost access to Origo, and will no longer be able to complete task(s), Please enter Origo to complete task(s)[View User]({{UserDetailViewUrl}})", OffboardingOverdueMailTemplate.Replace(System.Environment.NewLine, string.Empty));
        }
        [Fact]
        public void GetEmailTemplate_Norwegian()
        {
            //Arrange
            var language = "no";
            var resourceManger = new ResourceManager("CustomerServices.Resources.Customer", Assembly.GetAssembly(typeof(EmailService)));

            //Act
            var InvitationMailTemplate = resourceManger.GetString(InvitationMail.TemplateName, CultureInfo.CreateSpecificCulture(language));
            var OffboardingInitiatedWithBuyoutTemplate = resourceManger.GetString(OffboardingInitiatedWithBuyout.TemplateName, CultureInfo.CreateSpecificCulture(language));
            var OffboardingInitiatedWithoutBuyoutTemplate = resourceManger.GetString(OffboardingInitiatedWithoutBuyout.TemplateName, CultureInfo.CreateSpecificCulture(language));
            var OffboardingOverdueMailTemplate = resourceManger.GetString(OffboardingOverdueMail.TemplateName, CultureInfo.CreateSpecificCulture(language));

            //Assert
            Assert.NotNull(InvitationMailTemplate);
            Assert.NotNull(OffboardingInitiatedWithBuyoutTemplate);
            Assert.NotNull(OffboardingInitiatedWithoutBuyoutTemplate);
            Assert.NotNull(OffboardingOverdueMailTemplate);

            //.Replace(System.Environment.NewLine, string.Empty)) is used to remove \r\n or \n which leads to unit test failing in docker. 
            Assert.Equal("### Hello {{FirstName}}!Origo er en selvbetjeningsportal for å hjelpe deg og din bedrift med å holde kontroll over dine eiendeler.For at du skal begynne å bruke Origo, vennligst gå gjennom vår onboarding tour:[Start Onboarding]({{OrigoBaseUrl}})", InvitationMailTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("### Hello {{FirstName}}!Når det gjelder din offboarding, har du noen oppgaver å fullføre i Origo før du drar.1. Velg om du vil kjøpe ut eller returnere eiendelen til selskapet. Hvis du velger å kjøpe eiendelen, må dette gjøres før {{LastBuyoutDay}}, for å kunne gjøre lønnstrekk. Etter denne datoen vil kun retur være tilgjengelig. Besøk Origo for å se utkjøpsprisen.Vennligst følg trinnene i portalen.[Start Offboarding]({{MyPageLink}})", OffboardingInitiatedWithBuyoutTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("### Hello {{FirstName}}!Når det gjelder offboarding, har du noen oppgaver å fullføre i Origo før du drar.1. Returner eiendelene til selskapetFølg trinnene i portalen[Start Offboarding]({{MyPageLink}})", OffboardingInitiatedWithoutBuyoutTemplate.Replace(System.Environment.NewLine, string.Empty));
            Assert.Equal("### Hello!En bruker har fått statusen \"Offboarding Overdue\".**Navn**: {{UserName}} **Siste arbeidsdag**: {{LastWorkingDays}} Brukeren har nå mistet tilgangen til Origo, og vil ikke lenger kunne fullføre oppgave(r). Vennligst gå inn i Origo for å fullføre oppgave(r).[View User]({{UserDetailViewUrl}})", OffboardingOverdueMailTemplate.Replace(System.Environment.NewLine, string.Empty));
        }
        [Fact]
        public void GetEmailTemplate_Norwgian_UpperCaseLanguageCode()
        {
            //Arrange
            var resourceManger = new ResourceManager("CustomerServices.Resources.Customer", Assembly.GetAssembly(typeof(EmailService))!);
            //Act
            var OrigoInvitationNorwegianTemplate = resourceManger.GetString("OrigoInvitation", CultureInfo.CreateSpecificCulture("NO"));
            //Assert
            Assert.NotNull(OrigoInvitationNorwegianTemplate);
            //.Replace(System.Environment.NewLine, string.Empty)) is used to remove \r\n or \n which leads to unit test failing in docker.
            Assert.Equal("### Hello {{FirstName}}!Origo er en selvbetjeningsportal for å hjelpe deg og din bedrift med å holde kontroll over dine eiendeler.For at du skal begynne å bruke Origo, vennligst gå gjennom vår onboarding tour:[Start Onboarding]({{OrigoBaseUrl}})", OrigoInvitationNorwegianTemplate.Replace(System.Environment.NewLine, string.Empty));
        }

        [Fact]
        public void GetEmailTemplate_Norwgian_nb()
        {
            //Arrange
            var resourceManger = new ResourceManager("CustomerServices.Resources.Customer", Assembly.GetAssembly(typeof(EmailService))!);
            //Act
            var OrigoInvitationNorwegianTemplate = resourceManger.GetString("OrigoInvitation", CultureInfo.CreateSpecificCulture("nb"));
            //Assert
            Assert.NotNull(OrigoInvitationNorwegianTemplate);
            //.Replace(System.Environment.NewLine, string.Empty)) is used to remove \r\n or \n which leads to unit test failing in docker. 
            Assert.Equal("### Hello {{FirstName}}!Origo er en selvbetjeningsportal for å hjelpe deg og din bedrift med å holde kontroll over dine eiendeler.For at du skal begynne å bruke Origo, vennligst gå gjennom vår onboarding tour:[Start Onboarding]({{OrigoBaseUrl}})", OrigoInvitationNorwegianTemplate.Replace(System.Environment.NewLine, string.Empty));
        }
        [Fact]
        public void GetEmailTemplate_Swedish_sv()
        {
            //Arrange
            var resourceManger = new ResourceManager("CustomerServices.Resources.Customer", Assembly.GetAssembly(typeof(EmailService))!);
            //Act
            var OrigoInvitationSwedishTemplate = resourceManger.GetString("OrigoInvitation", CultureInfo.CreateSpecificCulture("sv"));
            //Assert
            Assert.NotNull(OrigoInvitationSwedishTemplate);
            //.Replace(System.Environment.NewLine, string.Empty)) is used to remove \r\n or \n which leads to unit test failing in docker. 
            Assert.Equal("### Hej {{FirstName}}!Origo är en självbetjäningsportal som hjälper dig och ditt företag att behålla kontrollen över dina tillgångar.För att du ska börja använda Origo, gå igenom vår onboarding tour:[Start Onboarding]({{OrigoBaseUrl}})", OrigoInvitationSwedishTemplate.Replace(System.Environment.NewLine, string.Empty));
        }
        [Fact]
        public void GetEmailTemplate_SE_ReturnsEnglish()
        {
            //Arrange
            var resourceManger = new ResourceManager("CustomerServices.Resources.Customer", Assembly.GetAssembly(typeof(EmailService))!);
            //Act
            var OrigoInvitationEnglishTemplate = resourceManger.GetString("OrigoInvitation", CultureInfo.CreateSpecificCulture("SE"));
            //Assert
            Assert.NotNull(OrigoInvitationEnglishTemplate);
            //.Replace(System.Environment.NewLine, string.Empty)) is used to remove \r\n or \n which leads to unit test failing in docker. 
            Assert.Equal("### Hello {{FirstName}}!Origo is a self service portal to help you and your business to keep control of your assets.In order for you to start using Origo, please go through our onboarding tour:[Start Onboarding]({{OrigoBaseUrl}})", OrigoInvitationEnglishTemplate.Replace(System.Environment.NewLine, string.Empty));
        }
        [Fact]
        public void GetEmailTemplate_DefaultToEnglish_WhenLanguageCodeIsNotOneWeHave()
        {
            //Arrange
            var resourceManger = new ResourceManager("CustomerServices.Resources.Customer", Assembly.GetAssembly(typeof(EmailService))!);
            //Act
            var OrigoInvitationEnglishTemplate = resourceManger.GetString("OrigoInvitation", CultureInfo.CreateSpecificCulture("FR"));
            //Arrange
            Assert.NotNull(OrigoInvitationEnglishTemplate);
            //.Replace(System.Environment.NewLine, string.Empty)) is used to remove \r\n or \n which leads to unit test failing in docker. 
            Assert.Equal("### Hello {{FirstName}}!Origo is a self service portal to help you and your business to keep control of your assets.In order for you to start using Origo, please go through our onboarding tour:[Start Onboarding]({{OrigoBaseUrl}})", OrigoInvitationEnglishTemplate.Replace(System.Environment.NewLine, string.Empty));
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
