using AutoMapper;
using Common.Configuration;
using Common.Utilities;
using HardwareServiceOrderServices.Configuration;
using HardwareServiceOrderServices.Email;
using HardwareServiceOrderServices.Infrastructure;
using HardwareServiceOrderServices.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;

namespace HardwareServiceOrder.UnitTests
{
    public class EmailServiceTests : HardwareServiceOrderServiceBaseTests
    {
        private readonly IEmailService _emailService;
        private readonly OrigoConfiguration _origoConfiguration;
        private readonly HardwareServiceOrderContext _hardwareServiceOrderContext;
        public EmailServiceTests() : base(new DbContextOptionsBuilder<HardwareServiceOrderContext>()

        .UseSqlite("Data Source=sqlitehardwareserviceorderservicetestsemail.db").Options)
        {
            var resourceManger = new ResourceManager("HardwareServiceOrderServices.Resources.HardwareServiceOrder", Assembly.GetAssembly(typeof(EmailService)));
            var emailOptions = Options.Create(new EmailConfiguration
            {

            });
            _origoConfiguration = new OrigoConfiguration
            {
                BaseUrl = "https://origov2dev.mytos.no",
                OrderPath = "/my-business/{0}/hardware-repair/{1}/view"
            };
            var origoOptions = Options.Create(_origoConfiguration);
            var flatDictionary = new FlatDictionary();

            _hardwareServiceOrderContext = new HardwareServiceOrderContext(ContextOptions);

            var mapper = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new EmailProfile());
            }).CreateMapper();

            _emailService = new EmailService(emailOptions, flatDictionary, resourceManger, mapper, origoOptions, _hardwareServiceOrderContext);
        }

        [Fact]
        public void GetEmailTemplate()
        {
            var resourceManger = new ResourceManager("HardwareServiceOrderServices.Resources.HardwareServiceOrder", Assembly.GetAssembly(typeof(EmailService)));

            var orderConfirmationEngTemplate = resourceManger.GetString("OrderConfirmationEmail", CultureInfo.CreateSpecificCulture("EN"));
            Assert.NotNull(orderConfirmationEngTemplate);
            var orderConfirmationNoTemplate = resourceManger.GetString("OrderConfirmationEmail", new CultureInfo("no"));
            Assert.NotNull(orderConfirmationNoTemplate);
        }

        [Fact]
        public async Task SendAssetRepairEmail()
        {
            var emails = await _emailService.SendAssetRepairEmailAsync(DateTime.Today.AddDays(-7), 200);
            Assert.NotNull(emails);
            Assert.Single(emails);

            emails.ForEach(email =>
            {
                Assert.Equal(email.OrderLink, string.Format($"{_origoConfiguration.BaseUrl}/{_origoConfiguration.OrderPath}", email.CustomerId, email.OrderId));
            });
        }

        [Fact]
        public async Task SendLoanDeviceEmail()
        {
            var emails = await _emailService.SendLoanDeviceEmailAsync(new List<int> { 200, 300 });
            Assert.NotNull(emails);
            Assert.Equal(2, emails.Count);
            Assert.Equal(2, await _hardwareServiceOrderContext.HardwareServiceOrders.CountAsync(m => m.IsReturnLoanDeviceEmailSent));
        }

        [Fact]
        public async Task SendAssetDiscardEmail()
        {
            var emails = await _emailService.SendOrderDiscardedEmailAsync(200);
            Assert.NotNull(emails);
            Assert.Single(emails);
            Assert.Equal(1, await _hardwareServiceOrderContext.HardwareServiceOrders.CountAsync(m => m.IsOrderDiscardedEmailSent));
        }
    }
}
