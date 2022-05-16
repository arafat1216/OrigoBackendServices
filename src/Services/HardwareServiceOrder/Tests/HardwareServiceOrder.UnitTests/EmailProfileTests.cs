using AutoMapper;
using HardwareServiceOrderServices.Email.Models;
using HardwareServiceOrderServices.Mappings;
using HardwareServiceOrderServices.Models;
using System;
using Xunit;

namespace HardwareServiceOrder.UnitTests
{
    public class EmailProfileTests
    {
        private readonly IMapper _mapper;

        public EmailProfileTests()
        {
            _mapper = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new EmailProfile());
            }).CreateMapper();
        }

        [Fact]
        public void HardwareServiceOrderToAssetRepairEmail()
        {
            var deliveryAddress = new DeliveryAddress("Recipient", "Description", "Address1", "Address2", "PostalCode", "City", "Country");
            var serviceTye = new ServiceType() { Id = 1 };
            var serviceStatus = new ServiceStatus() { };
            var serviceProvider = new ServiceProvider { OrganizationId = Guid.NewGuid() };
            var order = new HardwareServiceOrderServices.Models.HardwareServiceOrder(customerId: Guid.NewGuid(),assetLifecycleId: Guid.NewGuid(), deliveryAddress, "UserDescription", "externalLink", serviceTye, new ServiceStatus() { Id = 3 }, serviceProvider, new User(Guid.NewGuid(), "test@test.com", "UserName"), DateTime.Today.AddDays(-7));

            var assetRepairEmail = _mapper.Map<AssetRepairEmail>(order);
            Assert.NotNull(assetRepairEmail);
            Assert.Equal(order.OrderedBy.Name, assetRepairEmail.FirstName);
            Assert.Equal(order.OrderedBy.Email, assetRepairEmail.Recipient);
            Assert.Equal(order.ExternalProviderLink, assetRepairEmail.PackageSlipLink);
            Assert.Equal("Repair Reminder", assetRepairEmail.Subject);
            Assert.Equal(order.CreatedDate, assetRepairEmail.OrderDate);
            Assert.Equal(order.ExternalId, assetRepairEmail.OrderId);
            Assert.Equal(order.CustomerId, assetRepairEmail.CustomerId);
            Assert.Null(assetRepairEmail.OrderLink);
        }

        [Fact]
        public void HardwareServiceOrderToLoanDevice()
        {
            var deliveryAddress = new DeliveryAddress("Recipient", "Description", "Address1", "Address2", "PostalCode", "City", "Country");
            var serviceTye = new ServiceType() { Id = 1 };
            var serviceStatus = new ServiceStatus() { };
            var serviceProvider = new ServiceProvider { OrganizationId = Guid.NewGuid() };
            var order = new HardwareServiceOrderServices.Models.HardwareServiceOrder(customerId: Guid.NewGuid(), assetLifecycleId: Guid.NewGuid(), deliveryAddress, "UserDescription", "externalLink", serviceTye, new ServiceStatus() { Id = 3 }, serviceProvider, new User(Guid.NewGuid(), "test@test.com", "UserName"), DateTime.Today.AddDays(-7));

            var assetRepairEmail = _mapper.Map<LoanDeviceEmail>(order);
            Assert.NotNull(assetRepairEmail);
            Assert.Equal(order.OrderedBy.Name, assetRepairEmail.FirstName);
            Assert.Equal(order.OrderedBy.Email, assetRepairEmail.Recipient);
            Assert.Equal("Return Loan Device", assetRepairEmail.Subject);
        }

        [Fact]
        public void HardwareServiceOrderToAssetDiscardedEmail()
        {
            var deliveryAddress = new DeliveryAddress("Recipient", "Description", "Address1", "Address2", "PostalCode", "City", "Country");
            var serviceTye = new ServiceType() { Id = 1 };
            var serviceStatus = new ServiceStatus() { };
            var serviceProvider = new ServiceProvider { OrganizationId = Guid.NewGuid() };
            var order = new HardwareServiceOrderServices.Models.HardwareServiceOrder(customerId: Guid.NewGuid(), assetLifecycleId: Guid.NewGuid(), deliveryAddress, "UserDescription", "externalLink", serviceTye, new ServiceStatus() { Id = 3 }, serviceProvider, new User(Guid.NewGuid(), "test@test.com", "UserName"), DateTime.Today.AddDays(-7));

            var assetRepairEmail = _mapper.Map<AssetDiscardedEmail>(order);
            Assert.NotNull(assetRepairEmail);
            Assert.Equal(order.OrderedBy.Name, assetRepairEmail.FirstName);
            Assert.Equal(order.OrderedBy.Email, assetRepairEmail.Recipient);
            Assert.Equal("Replace Discarded Asset", assetRepairEmail.Subject);
        }
    }
}
