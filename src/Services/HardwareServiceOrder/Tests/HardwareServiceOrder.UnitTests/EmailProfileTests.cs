﻿using AutoMapper;
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
            var serviceType = new ServiceType() { Id = (int)ServiceTypeEnum.Recycle };
            var serviceStatus = new ServiceStatus() { Id = (int)ServiceStatusEnum.Registered };
            var serviceProvider = new ServiceProvider { OrganizationId = Guid.NewGuid() };
            var order = new HardwareServiceOrderServices.Models.HardwareServiceOrder(Guid.NewGuid(), new User(Guid.NewGuid(), "test@test.com", "UserName"), Guid.NewGuid(), "[AssetName]", deliveryAddress, "UserDescription", serviceProvider, Guid.NewGuid().ToString(), null, "externalLink", serviceType, serviceStatus, DateTime.Today.AddDays(-7));

            var assetRepairEmail = _mapper.Map<AssetRepairEmail>(order);
            Assert.NotNull(assetRepairEmail);
            Assert.Equal(order.OrderedBy.Name, assetRepairEmail.FirstName);
            Assert.Equal(order.OrderedBy.Email, assetRepairEmail.Recipient);
            Assert.Equal(order.ExternalServiceManagementLink, assetRepairEmail.PackageSlipLink);
            Assert.Equal("Repair Reminder", assetRepairEmail.Subject);
            Assert.Equal(order.CreatedDate, assetRepairEmail.OrderDate);
            Assert.Equal(order.ExternalId, assetRepairEmail.OrderId);
            Assert.Equal(order.CustomerId, assetRepairEmail.CustomerId);
            Assert.Null(assetRepairEmail.OrderLink);
        }

        [Fact]
        public void HardwareServiceOrderToOrderCancellationEmail()
        {
            var deliveryAddress = new DeliveryAddress("Recipient", "Description", "Address1", "Address2", "PostalCode", "City", "Country");
            var serviceType = new ServiceType() { Id = (int)ServiceTypeEnum.Remarketing };
            var serviceStatus = new ServiceStatus() { Id = (int)ServiceStatusEnum.Registered };
            var serviceProvider = new ServiceProvider { OrganizationId = Guid.NewGuid() };
            var order = new HardwareServiceOrderServices.Models.HardwareServiceOrder(Guid.NewGuid(), new User(Guid.NewGuid(), "test@test.com", "UserName"), Guid.NewGuid(), "[AssetName]", deliveryAddress, "UserDescription", serviceProvider, Guid.NewGuid().ToString(), null, "externalLink", serviceType, serviceStatus, DateTime.Today.AddDays(-7));

            var assetRepairEmail = _mapper.Map<OrderCancellationEmail>(order);
            Assert.NotNull(assetRepairEmail);
            Assert.Equal(order.OrderedBy.Name, assetRepairEmail.FirstName);
            Assert.Equal(order.OrderedBy.Email, assetRepairEmail.Recipient);
            Assert.Equal("Canceled Repair Order", assetRepairEmail.Subject);
            Assert.Equal(order.CreatedDate, assetRepairEmail.OrderDate);
            Assert.Equal(order.ExternalId, assetRepairEmail.OrderId);
            Assert.Equal(order.CustomerId, assetRepairEmail.CustomerId);
            Assert.Equal(order.AssetName, assetRepairEmail.AssetName);
            Assert.Equal(order.AssetLifecycleId, assetRepairEmail.AssetId);
            Assert.Equal(order.UserDescription, assetRepairEmail.FaultCategory);
            Assert.Equal($"{ServiceTypeEnum.Remarketing}", assetRepairEmail.RepairType);
            Assert.Null(assetRepairEmail.OrderLink);
        }

        [Fact]
        public void HardwareServiceOrderToLoanDevice()
        {
            var deliveryAddress = new DeliveryAddress("Recipient", "Description", "Address1", "Address2", "PostalCode", "City", "Country");
            var serviceType = new ServiceType() { Id = (int)ServiceTypeEnum.Recycle };
            var serviceStatus = new ServiceStatus() { Id = (int)ServiceStatusEnum.Registered };
            var serviceProvider = new ServiceProvider { OrganizationId = Guid.NewGuid() };
            var order = new HardwareServiceOrderServices.Models.HardwareServiceOrder(Guid.NewGuid(), new User(Guid.NewGuid(), "test@test.com", "UserName"), Guid.NewGuid(), "[AssetName]", deliveryAddress, "UserDescription", serviceProvider, Guid.NewGuid().ToString(), null, "externalLink", serviceType, serviceStatus, DateTime.Today.AddDays(-7));

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
            var serviceType = new ServiceType() { Id = (int)ServiceTypeEnum.Recycle };
            var serviceStatus = new ServiceStatus() { Id = (int)ServiceStatusEnum.Registered };
            var serviceProvider = new ServiceProvider { OrganizationId = Guid.NewGuid() };
            var order = new HardwareServiceOrderServices.Models.HardwareServiceOrder(Guid.NewGuid(), new User(Guid.NewGuid(), "test@test.com", "UserName"), Guid.NewGuid(), "[AssetName]", deliveryAddress, "UserDescription", serviceProvider, Guid.NewGuid().ToString(), null, "externalLink", serviceType, serviceStatus, DateTime.Today.AddDays(-7));

            var assetRepairEmail = _mapper.Map<AssetDiscardedEmail>(order);
            Assert.NotNull(assetRepairEmail);
            Assert.Equal(order.OrderedBy.Name, assetRepairEmail.FirstName);
            Assert.Equal(order.OrderedBy.Email, assetRepairEmail.Recipient);
            Assert.Equal("Replace Discarded Asset", assetRepairEmail.Subject);
        }


    }
}