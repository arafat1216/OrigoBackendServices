using Xunit;
using System.Resources;
using System.Reflection;
using HardwareServiceOrderServices.Email;
using System.Globalization;
using Microsoft.Extensions.Options;
using Common.Configuration;
using HardwareServiceOrderServices;
using Common.Utilities;
using Microsoft.EntityFrameworkCore;
using HardwareServiceOrderServices.Infrastructure;
using AutoMapper;
using HardwareServiceOrderServices.Mappings;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

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
            var emails = await _emailService.SendAssetRepairEmailAsync(DateTime.Today.AddDays(-7), new List<int> { 2, 3 });
            Assert.NotNull(emails);
            Assert.Equal(2, emails.Count);

            emails.ForEach(email =>
            {
                Assert.Equal(email.OrderLink, string.Format($"{_origoConfiguration.BaseUrl}/{_origoConfiguration.OrderPath}", email.CustomerId, email.OrderId));
            });
        }

        [Fact]
        public async Task SendLoanDeviceEmail()
        {
            var emails = await _emailService.SendLoanDeviceEmailAsync(new List<int> { 2, 3 });
            Assert.NotNull(emails);
            Assert.Equal(2, emails.Count);
            Assert.Equal(2, await _hardwareServiceOrderContext.HardwareServiceOrders.CountAsync(m => m.IsReturnLoanDeviceEmailSent));
        }

        [Fact]
        public async Task SendAssetDiscardEmail()
        {
            var emails = await _emailService.SendOrderDiscardedEmailAsync(2);
            Assert.NotNull(emails);
            Assert.Single(emails);
            Assert.Equal(1, await _hardwareServiceOrderContext.HardwareServiceOrders.CountAsync(m => m.IsOrderDiscardedEmailSent));
        }
    }
}
