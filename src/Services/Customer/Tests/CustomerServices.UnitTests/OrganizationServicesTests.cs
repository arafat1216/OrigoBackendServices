﻿using AutoMapper;
using Common.Logging;
using CustomerServices.Infrastructure;
using CustomerServices.Infrastructure.Context;
using CustomerServices.ServiceModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Common.Infrastructure;
using Xunit;
using CustomerServices.Email;
using CustomerServices.Email.Models;
//using System.Collections.Generic;

namespace CustomerServices.UnitTests
{
    public class OrganizationServicesTests : OrganizationServicesBaseTest
    {
        private readonly IMapper _mapper;
        private readonly OrganizationServices organizationServices;
        private readonly Mock<IEmailService> _emailServiceMock;

        public OrganizationServicesTests()
            : base(
                new DbContextOptionsBuilder<CustomerContext>()
                    // ReSharper disable once StringLiteralTypo
                    .UseSqlite("Data Source=sqlitecustomerunittests.db")
                    .Options
            )
        {
            var mappingConfig =
                new MapperConfiguration(mc => { mc.AddMaps(Assembly.GetAssembly(typeof(LocationDTO))); });
            _mapper = mappingConfig.CreateMapper();
            var apiRequesterServiceMock = new Mock<IApiRequesterService>();
            apiRequesterServiceMock.Setup(r => r.AuthenticatedUserId).Returns(CALLER_ID);
            var context = new CustomerContext(ContextOptions, apiRequesterServiceMock.Object);
            var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var emailMock = new Mock<IEmailService>();
            _emailServiceMock = emailMock;
            organizationServices = new OrganizationServices(Mock.Of<ILogger<OrganizationServices>>(), organizationRepository, _mapper, emailMock.Object);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task GetCompanyOne_CheckName()
        {
            // Act
            var organization = await organizationServices.GetOrganizationAsync(CUSTOMER_ONE_ID);

            // Assert
            Assert.Equal("COMPANY ONE", organization.Name);
            Assert.Equal("My Way 1", organization.Address.Street);
            Assert.Equal("1111", organization.Address.PostCode);
            Assert.Equal("My City", organization.Address.City);
            Assert.Equal("NO", organization.Address.Country);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task PutCompanyOne_null_values()
        {
            // Aarrange
            var orgData = new UpdateOrganizationDTO()
            {
                OrganizationId = CUSTOMER_ONE_ID,
                ParentId = null,
                PrimaryLocation = null,
                CallerId = Guid.Empty,
                Name = "Mytos",
                LastDayForReportingSalaryDeduction = 1
            };

            // Act
            var organization = await organizationServices.PutOrganizationAsync(orgData);

            // Assert 
            Assert.Equal("Mytos", organization.Name);
            Assert.Equal("", organization.OrganizationNumber);
            Assert.Null(organization.ParentId);
            Assert.True(organization.PrimaryLocation!.IsNull());
            Assert.Equal(CALLER_ID, organization.UpdatedBy);
            Assert.Equal("", organization.Address.Street);
            Assert.Equal("", organization.Address.PostCode);
            Assert.Equal("", organization.Address.City);
            Assert.Equal("", organization.Address.Country);
            Assert.Equal("", organization.ContactPerson.FirstName);
            Assert.Equal("", organization.ContactPerson.LastName);
            Assert.Equal("", organization.ContactPerson.Email);
            Assert.Equal("", organization.ContactPerson.PhoneNumber);
        }


        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task AddOrganization_WithoutAddOktaUsersSet_CheckDefaultValueSetToFalse()
        {
            // Act
            var organization = await organizationServices.AddOrganizationAsync(new NewOrganizationDTO
            {
                Name = "COMPANY NAME",
                OrganizationNumber = "999999999",
                Location = new LocationDTO(),
                Address = new AddressDTO(),
                ContactPerson = new ContactPersonDTO()
            });

            // Assert 
            Assert.False(organization.AddUsersToOkta);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task AddOrganization_WithAddOktaUsersSet_CheckValue()
        {
            // Act
            var organization = await organizationServices.AddOrganizationAsync(new NewOrganizationDTO
            {
                Name = "COMPANY NAME",
                OrganizationNumber = "999999999",
                Location = new LocationDTO(),
                Address = new AddressDTO(),
                ContactPerson = new ContactPersonDTO(),
                AddUsersToOkta = true
            });

            // Assert 
            Assert.True(organization.AddUsersToOkta);
        }
        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task AddOrganization_initializedStatus_BeforOnboarding()
        {
            // Act
            var organization = await organizationServices.AddOrganizationAsync(new NewOrganizationDTO
            {
                Name = "COMPANY NAME",
                OrganizationNumber = "999999999",
                Location = new LocationDTO(),
                Address = new AddressDTO(),
                ContactPerson = new ContactPersonDTO()
            });

            // Assert 
            Assert.Equal(Common.Enums.CustomerStatus.BeforeOnboarding,organization.Status);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task PutCompanyOne_partial_null_values()
        {
            // Arrange
            var orgData = new UpdateOrganizationDTO()
            {
                OrganizationId = CUSTOMER_ONE_ID,
                ParentId = null,
                PrimaryLocation = null,
                CallerId = Guid.Empty,
                Name = "name",
                Address = new AddressDTO()
                {
                    Street = "street",
                },
                ContactPerson = new ContactPersonDTO()
                {
                    FirstName = "FirstName"
                },
                LastDayForReportingSalaryDeduction = 1
            };

            // Act
            var organization = await organizationServices.PutOrganizationAsync(orgData);

            // Assert 
            Assert.Equal("name", organization.Name);
            Assert.Equal("", organization.OrganizationNumber);
            Assert.Null(organization.ParentId);
            Assert.True(organization.PrimaryLocation!.IsNull());
            Assert.Equal(CALLER_ID, organization.UpdatedBy);
            Assert.Equal("street", organization.Address.Street);
            Assert.Equal("", organization.Address.PostCode);
            Assert.Equal("", organization.Address.City);
            Assert.Equal("", organization.Address.Country);
            Assert.Equal("FirstName", organization.ContactPerson.FirstName);
            Assert.Equal("", organization.ContactPerson.LastName);
            Assert.Equal("", organization.ContactPerson.Email);
            Assert.Equal("", organization.ContactPerson.PhoneNumber);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task PutOrganization_WithAddOktaUsersSet_CheckValue()
        {
            // Arrange
            var orgData = new UpdateOrganizationDTO()
            {
                OrganizationId = CUSTOMER_ONE_ID,
                ParentId = null,
                PrimaryLocation = null,
                CallerId = Guid.Empty,
                Name = "name",
                Address = new AddressDTO()
                {
                    Street = "street"
                },
                ContactPerson = new ContactPersonDTO()
                {
                    FirstName = "FirstName"
                },
                LastDayForReportingSalaryDeduction = 1,
                AddUsersToOkta = true
            };

            // Act
            var organization = await organizationServices.PutOrganizationAsync(orgData);

            // Assert 
            Assert.True(organization.AddUsersToOkta);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task PatchOrganization_WithAddOktaUsersSet_CheckValue()
        {
            // Act
            var organization = await organizationServices.PatchOrganizationAsync(CUSTOMER_ONE_ID, null, null, Guid.Empty, "name", null, "street", null, null, null, "FirstName", null, null, null, 1, "", addUsersToOkta: true);

            // Assert 
            Assert.True(organization.AddUsersToOkta);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task GetOrganization_WithAddOktaUsersSet_CheckValue()
        {
            // Arrange
            var organization = await organizationServices.AddOrganizationAsync(new NewOrganizationDTO
            {
                Name = "COMPANY NAME",
                OrganizationNumber = "999999999",
                Location = new LocationDTO(),
                Address = new AddressDTO(),
                ContactPerson = new ContactPersonDTO(),
                IsCustomer = true,
                AddUsersToOkta = true
            });

            // Act
            var addedOrganization = await organizationServices.GetOrganizationAsync(organization.OrganizationId);

            // Assert 
            Assert.True(addedOrganization!.AddUsersToOkta);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task PatchCompanyOne__null_values()
        {
            // Act
            var organization = await organizationServices.PatchOrganizationAsync(CUSTOMER_ONE_ID, null, null, Guid.Empty, null, null, null, null, null, null, null, null, null, null, 1, "");
            
            // Assert 
            Assert.Equal("COMPANY ONE", organization.Name);
            Assert.Equal("999888777", organization.OrganizationNumber);
            Assert.Null(organization.ParentId);
            Assert.NotNull(organization.PrimaryLocation);
            Assert.Equal(Guid.Empty, organization.UpdatedBy);
            Assert.Equal("My Way 1", organization.Address.Street);
            Assert.Equal("1111", organization.Address.PostCode);
            Assert.Equal("My City", organization.Address.City);
            Assert.Equal("NO", organization.Address.Country);
            Assert.Equal("JOHN", organization.ContactPerson.FirstName);
            Assert.Equal("DOE", organization.ContactPerson.LastName);
            Assert.Equal("john.doe@example.com", organization.ContactPerson.Email);
            Assert.Equal("99999999", organization.ContactPerson.PhoneNumber);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task PatchCompanyOne_partial_null_values()
        {
            // Act
            var organization = await organizationServices.PatchOrganizationAsync(CUSTOMER_ONE_ID, null, null, Guid.Empty, "name", null, "street", null, null, null, null, "Paavola", null, null, 1, "");

            // Assert 
            Assert.Equal("name", organization.Name);
            Assert.Equal("999888777", organization.OrganizationNumber);
            Assert.Null(organization.ParentId);
            Assert.NotNull(organization.PrimaryLocation);
            Assert.Equal(CALLER_ID, organization.UpdatedBy);
            Assert.Equal("street", organization.Address.Street);
            Assert.Equal("1111", organization.Address.PostCode);
            Assert.Equal("My City", organization.Address.City);
            Assert.Equal("NO", organization.Address.Country);
            Assert.Equal("JOHN", organization.ContactPerson.FirstName);
            Assert.Equal("Paavola", organization.ContactPerson.LastName);
            Assert.Equal("john.doe@example.com", organization.ContactPerson.Email);
            Assert.Equal("99999999", organization.ContactPerson.PhoneNumber);
        }
        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task InitiateOnboardingAsync_CallEmailOnlyOnUserStatusNotInvited()
        {
            // Act
            var organization = await organizationServices.InitiateOnboardingAsync(CUSTOMER_ONE_ID);
            // Arrange
            _emailServiceMock.Verify(email => email.InvitationEmailToUserAsync(It.IsAny<InvitationMail>(), It.IsAny<string>()), Times.Exactly(2));
        }
        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task GetOrganizationByTechstepCustomerIdAsync_ReturnsCustomer()
        {
            // Act
            var context = new CustomerContext(ContextOptions, null);
            var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var organization = await organizationRepository.GetOrganizationByTechstepCustomerIdAsync(123456789);
            // Assert
            Assert.Equal(CUSTOMER_FIVE_ID,organization?.OrganizationId);
        }
    }
}