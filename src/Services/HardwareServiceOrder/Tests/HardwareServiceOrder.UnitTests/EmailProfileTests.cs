using AutoMapper;
using HardwareServiceOrderServices.Email.Models;
using HardwareServiceOrderServices.Mappings;
using HardwareServiceOrderServices.Models;
using System;
using System.Collections.Generic;
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
            var deliveryAddress = new DeliveryAddress(RecipientTypeEnum.Personal, "Recipient", "Address1", "Address2", "PostalCode", "City", "Country");
            var contactDetails = new ContactDetails(Guid.NewGuid(), "FirstName", "LastName", "test@test.com", "PhoneNumber");
            AssetInfo assetInfo = new("[AssetBrand]", "[AssetModel]", new HashSet<string>() { "527127734377463" }, "[SerialNumber]", DateOnly.Parse("2020-01-01"), null);

#pragma warning disable CS0618 // Type or member is obsolete
            var order = new HardwareServiceOrderServices.Models.HardwareServiceOrder(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                1,
                assetInfo,
                "UserDescription",
                contactDetails,
                deliveryAddress,
                (int)ServiceTypeEnum.Recycle,
                (int)ServiceStatusEnum.Registered,
                (int)ServiceProviderEnum.ConmodoNo,
                Guid.NewGuid().ToString(),
                null,
                "externalLink",
                new List<ServiceEvent>(),
                Guid.NewGuid(),
                DateTime.Today.AddDays(-7)
            );
#pragma warning restore CS0618 // Type or member is obsolete

            var assetRepairEmail = _mapper.Map<AssetRepairEmail>(order);
            Assert.NotNull(assetRepairEmail);
            Assert.Equal(order.Owner.FirstName, assetRepairEmail.FirstName);
            Assert.Equal(order.Owner.Email, assetRepairEmail.Recipient);
            Assert.Equal(order.ExternalServiceManagementLink, assetRepairEmail.PackageSlipLink);
            Assert.Equal(order.DateCreated, assetRepairEmail.OrderDate);
            Assert.Equal(order.ExternalId, assetRepairEmail.OrderId);
            Assert.Equal(order.CustomerId, assetRepairEmail.CustomerId);
            Assert.Null(assetRepairEmail.OrderLink);
        }

        [Fact]
        public void HardwareServiceOrderToOrderCancellationEmail()
        {
            var deliveryAddress = new DeliveryAddress(RecipientTypeEnum.Personal, "Recipient", "Address1", "Address2", "PostalCode", "City", "Country");
            var contactDetails = new ContactDetails(Guid.NewGuid(), "FirstName", "LastName", "test@test.com", "PhoneNumber");

            var serviceType = new ServiceType() { Id = (int)ServiceTypeEnum.Remarketing };
            var serviceStatus = new ServiceStatus() { Id = (int)ServiceStatusEnum.Registered };
            var serviceProvider = new ServiceProvider(Guid.NewGuid(), "ServiceProviderName");

            var order = new HardwareServiceOrderServices.Models.HardwareServiceOrder(
                Guid.NewGuid(),
                contactDetails,
                Guid.NewGuid(),
                deliveryAddress,
                "UserDescription",
                serviceProvider,
                Guid.NewGuid().ToString(),
                null,
                "externalLink",
                serviceType,
                serviceStatus,
                DateTime.Today.AddDays(-7)
            );

            var assetRepairEmail = _mapper.Map<OrderCancellationEmail>(order);
            Assert.NotNull(assetRepairEmail);
            Assert.Equal(order.Owner.FirstName, assetRepairEmail.FirstName);
            Assert.Equal(order.Owner.Email, assetRepairEmail.Recipient);
            Assert.Equal(order.DateCreated, assetRepairEmail.OrderDate);
            Assert.Equal(order.ExternalId, assetRepairEmail.OrderId);
            Assert.Equal(order.CustomerId, assetRepairEmail.CustomerId);
            Assert.Equal(order.AssetLifecycleId, assetRepairEmail.AssetId);
            Assert.Equal(order.UserDescription, assetRepairEmail.FaultCategory);
            Assert.Equal($"{ServiceTypeEnum.Remarketing}", assetRepairEmail.RepairType);
            Assert.Null(assetRepairEmail.OrderLink);
        }

        [Fact]
        public void HardwareServiceOrderToLoanDevice()
        {
            var deliveryAddress = new DeliveryAddress(RecipientTypeEnum.Personal, "Recipient", "Address1", "Address2", "PostalCode", "City", "Country");
            var contactDetails = new ContactDetails(Guid.NewGuid(), "FirstName", "LastName", "test@test.com", "PhoneNumber");

            var serviceType = new ServiceType() { Id = (int)ServiceTypeEnum.Recycle };
            var serviceStatus = new ServiceStatus() { Id = (int)ServiceStatusEnum.Registered };
            var serviceProvider = new ServiceProvider(Guid.NewGuid(), "ServiceProviderName");
            var order = new HardwareServiceOrderServices.Models.HardwareServiceOrder(
                Guid.NewGuid(),
                contactDetails,
                Guid.NewGuid(),
                deliveryAddress,
                "UserDescription",
                serviceProvider,
                Guid.NewGuid().ToString(),
                null,
                "externalLink",
                serviceType,
                serviceStatus,
                DateTime.Today.AddDays(-7)
            );

            var assetRepairEmail = _mapper.Map<LoanDeviceEmail>(order);
            Assert.NotNull(assetRepairEmail);
            Assert.Equal(order.Owner.FirstName, assetRepairEmail.FirstName);
            Assert.Equal(order.Owner.Email, assetRepairEmail.Recipient);
        }

        [Fact]
        public void HardwareServiceOrderToAssetDiscardedEmail()
        {
            var deliveryAddress = new DeliveryAddress(RecipientTypeEnum.Personal, "Recipient", "Address1", "Address2", "PostalCode", "City", "Country");
            var contactDetails = new ContactDetails(Guid.NewGuid(), "FirstName", "LastName", "test@test.com", "PhoneNumber");

            var serviceType = new ServiceType() { Id = (int)ServiceTypeEnum.Recycle };
            var serviceStatus = new ServiceStatus() { Id = (int)ServiceStatusEnum.Registered };
            var serviceProvider = new ServiceProvider(Guid.NewGuid(), "ServiceProviderName");
            var order = new HardwareServiceOrderServices.Models.HardwareServiceOrder(
                Guid.NewGuid(),
                contactDetails,
                Guid.NewGuid(),
                deliveryAddress,
                "UserDescription",
                serviceProvider,
                Guid.NewGuid().ToString(),
                null,
                "externalLink",
                serviceType,
                serviceStatus,
                DateTime.Today.AddDays(-7)
            );

            var assetRepairEmail = _mapper.Map<AssetDiscardedEmail>(order);
            Assert.NotNull(assetRepairEmail);
            Assert.Equal(order.Owner.FirstName, assetRepairEmail.FirstName);
            Assert.Equal(order.Owner.Email, assetRepairEmail.Recipient);
        }


    }
}
